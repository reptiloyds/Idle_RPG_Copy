using System;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using R3;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model
{
    public class ChatConditions : IDisposable
    {
        private readonly Chat _chat;
        private readonly Character _character;
        private readonly MainMode _mainMode;
        private readonly ResourceModel _resource;
        private readonly ITranslator _translator;
        private readonly CompositeDisposable _disposable = new();
        private ChatConditionData _data;
        private readonly ReactiveProperty<string> _description = new();

        private readonly string _notEnoughResourceCondition;
        private string _charLevelCondition;
        private string _mainModeCondition;

        private bool HasMainModeCondition => _data.StageLevel > 0 && _data.StageId >= 0;

        public ReadOnlyReactiveProperty<string> Description => _description;
        public event Action OnUpdated;

        public ChatConditions(Chat chat, Character character, MainMode mainMode, ResourceService resourceService, ITranslator translator)
        {
            _chat = chat;
            _character = character;
            _mainMode = mainMode;
            _translator = translator;
            _resource = resourceService.GetResource(ResourceType.ChatTicket);
            _disposable.Add(_chat.ConversationConfig.Subscribe(value => SetupData(value.Condition)));
            
            _notEnoughResourceCondition = _translator.Translate($"{TranslationConst.NotEnoughPrefix}{_resource.Type}");
        }

        private void SetupData(ChatConditionData data)
        {
            if (_data != null) 
                ClearData();
            _data = data;
            if (_data.Price > 0) 
                _resource.OnChange += OnResourceChange;
            if (_data.CharacterLevel > 0)
            {
                _character.OnLevelUp += OnCharacterLevelUp;
                _charLevelCondition = string.Format(_translator.Translate(TranslationConst.ChatCharLevelCondition), _data.CharacterLevel);  
            }
            if (HasMainModeCondition)
            {
                var stageName = _mainMode.GetFormattedStageName(_data.StageId);
                var stageNumber = _mainMode.GetStageNumber(_data.StageId);
                _mainMode.OnLevelEntered += OnMainModeEntered;
                _mainModeCondition = string.Format(_translator.Translate(TranslationConst.ChatMainModeCondition), stageName, stageNumber, _data.StageLevel);  
            }
            UpdateState();
        }

        private void OnResourceChange() => 
            UpdateState();

        private void OnCharacterLevelUp(Character character) => 
            UpdateState();

        private void OnMainModeEntered() => 
            UpdateState();

        private void ClearData()
        {
            if (_data.Price > 0) 
                _resource.OnChange -= OnResourceChange;
            if (_data.CharacterLevel > 0) 
                _character.OnLevelUp -= OnCharacterLevelUp;
            if (HasMainModeCondition) 
                _mainMode.OnLevelEntered -= OnMainModeEntered;

            _data = null;
        }

        public (ResourceType type, int amount) GetPrice() => 
            _data == null ? (ResourceType.ChatTicket, 0) : (ResourceType.ChatTicket, _data.Price);

        public bool IsAllConditionCompleted()
        {
            if (_data == null) return true;
            if (_data.Price > _resource.Value) return false;
            if (_data.CharacterLevel > _character.Level) return false;
            if (_data.StageId > _mainMode.Id)
                return false;
            if (_data.StageId == _mainMode.Id)
                return _data.StageLevel <= _mainMode.Level;
            return true;
        }

        private void UpdateState()
        {
            _description.Value = GetFormattedCondition();
            OnUpdated?.Invoke();
        }

        private string GetFormattedCondition()
        {
            if (_data.Price > _resource.Value)
                return _notEnoughResourceCondition;
            if (_data.CharacterLevel > _character.Level)
                return _charLevelCondition;
            if (_data.StageId > _mainMode.Id)
                return _mainModeCondition;
            if (_data.StageId == _mainMode.Id && _data.StageLevel > _mainMode.Level)
                return _mainModeCondition;
            return string.Empty;
        }

        public void Dispose() => 
            _disposable.Dispose();
    }
}