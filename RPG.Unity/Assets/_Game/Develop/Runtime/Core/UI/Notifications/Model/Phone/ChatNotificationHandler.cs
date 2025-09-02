using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Type;
using PleasantlyGames.RPG.Runtime.Core.Phone.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Pool;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Phone
{
    public class ChatNotificationHandler : IDisposable
    {
        [Inject] private ChatService _chatService;
        [Inject] private NotificationConfiguration _configuration;
        [Inject] private IWindowService _windowService;

        private readonly Dictionary<ChatCharacter, Notification> _chatNotifications = new();
        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private readonly CompositeDisposable _disposable = new();

        private readonly string _windowType = nameof(PhoneWindow);
        private PhoneWindow _phoneWindow;

        public Notification ChatIconNotification { get; private set; }

        public ChatNotificationHandler(ObjectPoolWithParent<NotificationView> pool) =>
            _pool = pool;

        public void Initialize()
        {
            var phoneSetup = _configuration.PhoneNotificationSetup;
            ChatIconNotification = new Notification(_pool, phoneSetup.IconSetup, phoneSetup.ImageSetup);
            foreach (var character in _chatService.Characters)
            {
                var chatNotification = new Notification(_pool, phoneSetup.ChatSetup, phoneSetup.ImageSetup);
                _chatNotifications.Add(character, chatNotification);
                if (!character.IsUnlocked) 
                    character.OnUnlocked += OnCharacterUnlocked;
                character.Chat.State
                    .Subscribe(_ => CheckChats())
                    .AddTo(_disposable);
                character.Conditions.OnUpdated += OnConditionsUpdated;
                ChatIconNotification.AppendChild(chatNotification);
            }

            var isWindowExist = _windowService.IsExist<PhoneWindow>();

            if (isWindowExist)
                HandleWindowAsync().Forget();
            else
                _windowService.OnCreate += OnWindowCreated;

            CheckChats();
        }

        private void OnConditionsUpdated() => 
            CheckChats();

        private void OnCharacterUnlocked(ChatCharacter character)
        {
            character.OnUnlocked -= OnCharacterUnlocked;
            CheckChat(character);
        }

        public void Dispose()
        {
            foreach (var character in _chatService.Characters)
            {
                character.Conditions.OnUpdated -= OnConditionsUpdated;
                if(character.IsUnlocked) continue;
                character.OnUnlocked -= OnCharacterUnlocked;
            }

            _disposable?.Dispose();
        }

        private async void OnWindowCreated(string windowType)
        {
            if (!string.Equals(windowType, _windowType)) return;

            _windowService.OnCreate -= OnWindowCreated;
            await HandleWindowAsync();
        }

        private async UniTask HandleWindowAsync()
        {
            _phoneWindow ??= await _windowService.GetAsync<PhoneWindow>(false);
            ChatIconNotification.SetParent(_phoneWindow.ChatButton.transform);

            foreach (var kvp in _chatNotifications)
            {
                var preview = _phoneWindow.ChatPage.GetPreviewByModel(kvp.Key);
                kvp.Value.SetParent(preview.transform);
            }
        }

        private void CheckChats()
        {
            foreach (var character in _chatService.Characters) 
                CheckChat(character);
        }

        private void CheckChat(ChatCharacter character)
        {
            if (!_chatNotifications.TryGetValue(character, out var notification))
                return;
            if (!character.IsUnlocked)
            {
                notification.Disable();
                return;
            }
            if (character.Chat.State.CurrentValue is ChatState.Completed or ChatState.InProcess)
                notification.Disable();
            else if (character.Conditions.IsAllConditionCompleted())
                notification.Enable();
            else
                notification.Disable();
        }
    }
}