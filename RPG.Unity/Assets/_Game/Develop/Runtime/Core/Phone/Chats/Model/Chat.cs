using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Definitions;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Save;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Type;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using R3;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model
{
    public class Chat : IDisposable
    {
        private readonly ChatDefinition _definition;
        private readonly TimeService _timeService;
        private readonly IAudioService _audioService;
        private readonly GalleryService _galleryService;
        private readonly List<ChatSheet.Row> _configs;
        private readonly ChatData _data;
        private readonly string _characterId;
        private readonly ReactiveProperty<MessageData> _lastMessage = new();
        private readonly ReactiveProperty<DateTime> _lastMessageTime = new();
        private readonly CompositeDisposable _initializeDisposable = new();
        private readonly ReactiveProperty<ChatState> _state = new();

        private readonly List<ChatSheet.Elem> _stepMessages = new(5);
        private readonly List<string> _playerVariants = new();
        private readonly ReactiveProperty<ChatSheet.Row> _conversationConfig = new();
        private readonly ReactiveProperty<MessageData> _typingMessage = new();
        private List<MessageData> _postedMessages;

        public ReadOnlyReactiveProperty<ChatSheet.Row> ConversationConfig => _conversationConfig;
        public ReadOnlyReactiveProperty<MessageData> LastMessage => _lastMessage;
        public ReadOnlyReactiveProperty<DateTime> LastMessageTime => _lastMessageTime;
        public IReadOnlyList<MessageData> PostedMessages => _postedMessages;
        public IReadOnlyList<string> PlayerVariants => _playerVariants;
        public ReadOnlyReactiveProperty<ChatState> State => _state;
        public ReadOnlyReactiveProperty<MessageData> TypingMessage => _typingMessage;
        
        public string Id { get; private set; }

        public event Action OnPlayerVariantsOffered;
        public event Action<MessageData> OnMessagePosted;
        public event Action OnChatCompleted;

        public Chat(List<ChatSheet.Row> configs, ChatData data, string characterId,
            ChatDefinition definition, TimeService timeService, IAudioService audioService,
            GalleryService galleryService)
        {
            _configs = configs;
            _data = data;
            _characterId = characterId;
            _definition = definition;
            _timeService = timeService;
            _audioService = audioService;
            _galleryService = galleryService;
        }

        public void Initialize()
        {
            if (_data.Messages.Count > 0)
                _lastMessage.Value = _data.Messages[^1];
            _lastMessageTime.Value = _data.LastMessageTime;
            _lastMessageTime
                .Subscribe(value => _data.LastMessageTime = value)
                .AddTo(_initializeDisposable);

            _state.Value = _data.ChatState;
            _state
                .Subscribe(value => _data.ChatState = value)
                .AddTo(_initializeDisposable);

            _postedMessages = new List<MessageData>(_data.Messages);

            UpdateDialogueConfig();

            switch (_state.Value)
            {
                case ChatState.Awaiting:
                    break;
                case ChatState.InProcess:
                    if (_data.Step > 0)
                        ContinueConversation();
                    else
                        EndConversation();
                    break;
                case ChatState.Completed:
                    break;
            }
        }

        public void StartConversation()
        {
            _state.Value = ChatState.InProcess;
            if (_data.Step < 1)
                _data.Step = 1;

            ContinueConversation();
        }

        public void ChoosePlayerVariant(string variant)
        {
            _playerVariants.Clear();
            ChatSheet.Elem chosenMessage = null;
            foreach (var message in _stepMessages)
            {
                if (!string.Equals(message.Variant, variant)) continue;
                chosenMessage = message;
                break;
            }

            if (chosenMessage == null)
            {
                Logger.LogError("Chosen message not found");
                return;
            }

            _stepMessages.Clear();
            _stepMessages.Add(chosenMessage);

            PrintMessages();
        }

        private void EndConversation()
        {
            OnChatCompleted?.Invoke();
            
            if (_configs.IndexOf(_conversationConfig.Value) == _configs.Count - 1)
                CompleteChat();
            else
                NextConversation();
        }

        private void NextConversation()
        {
            _data.Сonversation++;
            _state.Value = ChatState.Awaiting;
            UpdateDialogueConfig();
        }

        private void CompleteChat()
        {
            _state.Value = ChatState.Completed;
        }

        private void ContinueConversation()
        {
            _stepMessages.Clear();
            foreach (var messageConfig in _conversationConfig.Value)
            {
                if (messageConfig.Step != _data.Step) continue;
                _stepMessages.Add(messageConfig);
            }

            if (_stepMessages.Count == 0)
            {
                Logger.LogError("Step messages not found");
                return;
            }

            var actor = _stepMessages[0].Actor;
            switch (actor)
            {
                case ActorType.Player:
                    HandlePlayerActor();
                    break;
                case ActorType.Converser:
                    HandleConverserActor();
                    break;
            }
        }

        private void HandlePlayerActor()
        {
            foreach (var stepMessage in _stepMessages)
            {
                if (string.IsNullOrEmpty(stepMessage.Variant)) continue;
                _playerVariants.Add(stepMessage.Variant);
            }

            if (_playerVariants.Count > 0)
                OnPlayerVariantsOffered?.Invoke();
            else
                PrintMessages();
        }

        private void HandleConverserActor() => PrintMessages();

        private void PrintMessages()
        {
            _data.Step = _stepMessages[^1].NextStep;
            foreach (var message in _stepMessages)
            {
                _data.Messages.Add(new MessageData()
                {
                    Actor = message.Actor,
                    Type = message.MessageType,
                    Key = message.MessageKey,
                });
            }

            TypeMessage().Forget();
        }

        private async UniTaskVoid TypeMessage()
        {
            var message = _stepMessages[0];
            _typingMessage.Value = message.ConvertToData();
            switch (message.MessageType)
            {
                case MessageType.Text:
                    await UniTask.Delay(TimeSpan.FromSeconds(_definition.PrintDelay), ignoreTimeScale: true);
                    break;
                case MessageType.Photo:
                    var delayTask = UniTask.Delay(TimeSpan.FromSeconds(_definition.PrintDelay), ignoreTimeScale: true);
                    _galleryService.UnlockPhoto(message.MessageKey);
                    var warmUpTask = _galleryService.WarmUpLowQualityAsync(message.MessageKey);
                    await UniTask.WhenAll(delayTask, warmUpTask);
                    break;
            }

            _typingMessage.Value = default;
            PostMessage();
        }

        private void PostMessage()
        {
            var message = _stepMessages[0];
            _stepMessages.RemoveAt(0);
            var messageData = message.ConvertToData();
            _postedMessages.Add(messageData);
            _lastMessage.Value = messageData;
            _lastMessageTime.Value = _timeService.Now();
            _audioService.CreateLocalSound(UI_Effect.UI_ChatMessage).Play();
            OnMessagePosted?.Invoke(messageData);

            if (_stepMessages.Count == 0)
            {
                if (_data.Step > 0)
                    ContinueConversation();
                else
                    EndConversation();
            }
            else
                TypeMessage().Forget();
        }

        private void UpdateDialogueConfig()
        {
            var conversationKey = $"{_characterId}_{_data.Сonversation}";
            Id = conversationKey;
            foreach (var config in _configs)
            {
                if (!string.Equals(config.Id, conversationKey)) continue;
                _conversationConfig.Value = config;
                break;
            }
        }

        void IDisposable.Dispose() =>
            _initializeDisposable?.Dispose();
    }
}