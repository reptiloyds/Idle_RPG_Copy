using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Definitions;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Save;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model
{
    public class ChatService : IDisposable
    {
        [Inject] private ChatDataProvider _dataProvider;
        [Inject] private BalanceContainer _balance;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private CharacterService _characterService;
        [Inject] private ChatDefinition _definition;
        [Inject] private TimeService _timeService;
        [Inject] private MainMode _mainMode;
        [Inject] private ITranslator _translator;
        [Inject] private ResourceService _resourceService;
        [Inject] private IAudioService _audioService;
        [Inject] private GalleryService _galleryService;

        private DialogueDataContainer _data;
        private ChatCharactersSheet _charactersSheet;
        private ChatSheet _chatSheet;

        private readonly List<ChatCharacter> _characters = new();
        public IReadOnlyList<ChatCharacter> Characters => _characters;

        public event Action<string> OnChatCompleted;
        public event Action<string> OnAnswersShown;
        
        public void Initialize()
        {
            _data = _dataProvider.GetData();
            _charactersSheet = _balance.Get<ChatCharactersSheet>();
            _chatSheet = _balance.Get<ChatSheet>();
            CreateModels();
        }

        private void CreateModels()
        {
            foreach (var dataKvp in _data.ChatCharacters)
            {
                var characterConfig = _charactersSheet[dataKvp.Key];
                var sprite = _spriteProvider.GetSprite(Asset.ChatAvatarAtlas, characterConfig.ImageName);
                var character = _characterService.GetCharacter(dataKvp.Key);
                var chatCharacter = new ChatCharacter(characterConfig, dataKvp.Value, sprite, character,
                    _chatSheet, _definition, _timeService, _mainMode, _translator, _resourceService, _audioService, _galleryService);
                chatCharacter.Initialize();
                _characters.Add(chatCharacter);
                chatCharacter.OnChatCompleted += RaiseOnChatCompleted;
                chatCharacter.OnAnswersShown += RaiseOnAnswersShown;
            }
        }

        private void RaiseOnAnswersShown(string id)
        {
            OnAnswersShown?.Invoke(id);
        }

        private void RaiseOnChatCompleted(string id)
        {
            OnChatCompleted?.Invoke(id);
        }

        public void Dispose()
        {
            foreach (var character in _characters)
            {
                character.OnChatCompleted -= RaiseOnChatCompleted;
                character.OnAnswersShown -= RaiseOnAnswersShown;
                character.Dispose();
            }
        }
    }
}
