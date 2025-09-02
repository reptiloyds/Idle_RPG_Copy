
using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Definitions;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Save;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Type;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using R3;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model
{
    public class ChatCharacter : IDisposable
    {
        private readonly ChatCharactersSheet.Row _config;
        private readonly ChatCharacterData _data;
        private readonly Character _character;
        private readonly ChatSheet _chatSheet;
        private readonly ChatDefinition _definition;
        private readonly TimeService _timeService;
        private readonly MainMode _mainMode;
        private readonly ITranslator _translator;
        private readonly ResourceService _resourceService;
        private readonly IAudioService _audioService;
        private readonly GalleryService _galleryService;
        private readonly CompositeDisposable _disposable = new();

        public Sprite Sprite { get; }
        public string Name => _character.FormattedName;
        public Chat Chat { get; private set; }
        public ChatConditions Conditions { get; private set; }
        public bool IsUnlocked => _character.IsOwned;
        public event Action<ChatCharacter> OnUnlocked;
        public event Action<string> OnChatCompleted;
        public event Action<string> OnAnswersShown;

        public ChatCharacter(ChatCharactersSheet.Row config, ChatCharacterData data, Sprite sprite, 
            Character character, ChatSheet chatSheet, ChatDefinition definition, TimeService timeService,
            MainMode mainMode, ITranslator translator, ResourceService resourceService, IAudioService audioService,
            GalleryService galleryService)
        {
            _config = config;
            _data = data;
            Sprite = sprite;
            _character = character;
            _chatSheet = chatSheet;
            _definition = definition;
            _timeService = timeService;
            _mainMode = mainMode;
            _translator = translator;
            _resourceService = resourceService;
            _audioService = audioService;
            _galleryService = galleryService;
        }

        public void Initialize()
        {
            var dialogueConfigs = GetDialogueConfigsFor(_config.Id);
            Chat = new Chat(dialogueConfigs, _data.ChatData, _config.Id, _definition, _timeService, _audioService, _galleryService);
            Chat.Initialize();

            Chat.OnPlayerVariantsOffered += RaiseOnVariantsOffered;
            Chat.OnChatCompleted += RaiseOnChatCompleted;
            
            Conditions = new ChatConditions(Chat, _character, _mainMode, _resourceService, _translator);
            
            if (!_character.IsOwned) 
                _character.OnOwned += OnCharacterOwned;
        }

        private void RaiseOnChatCompleted()
        {
            OnChatCompleted?.Invoke(Chat.Id);
        }

        private void RaiseOnVariantsOffered()
        {
            OnAnswersShown?.Invoke(Chat.Id);
        }

        private List<ChatSheet.Row> GetDialogueConfigsFor(string characterId)
        {
            List<ChatSheet.Row> result = new();
            foreach (var config in _chatSheet)
            {
                if (config.Id.Contains(characterId)) 
                    result.Add(config);
            }
            return result;
        }

        private void OnCharacterOwned(Character character)
        {
            character.OnOwned -= OnCharacterOwned;
            OnUnlocked?.Invoke(this);
        }

        public void Dispose()
        {
            _disposable.Dispose();
            Conditions.Dispose();
            Chat.OnPlayerVariantsOffered -= RaiseOnVariantsOffered;
            Chat.OnChatCompleted -= RaiseOnChatCompleted;
        }
    }
}