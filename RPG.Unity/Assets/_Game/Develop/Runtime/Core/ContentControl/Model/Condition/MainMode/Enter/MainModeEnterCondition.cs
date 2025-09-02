using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Sheet;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition.MainMode.Enter
{
    public class MainModeEnterCondition : UnlockCondition
    {
        private MainModeEnterConditionData _conditionData;
        private readonly GameMode.Model.Main.MainMode _mainMode;
        private int _targetLevels;

        public MainModeEnterCondition(ContentSheet.Row config, ITranslator translator, GameMode.Model.Main.MainMode mainMode) : base(config, translator) => 
            _mainMode = mainMode;

        public override string GetDescription()
        {
            var localizedDescription = Translator.Translate(LocalizationKey);
            var localizedPeriod = _mainMode.GetFormattedStageName(_conditionData.Id);
            return string.Format(localizedDescription, localizedPeriod, _mainMode.GetStageNumber(_conditionData.Id), _conditionData.Level);
        }

        public override void Initialize()
        {
            base.Initialize();

            _targetLevels = _mainMode.GetLevelsTo(_conditionData.Id, _conditionData.Level);
            _mainMode.OnLevelEntered += Check;
            Check();
        }

        protected override void Clear()
        {
            base.Clear();
            _mainMode.OnLevelEntered -= Check;
        }

        protected override void CreateData(string dataJson) => 
            _conditionData = JsonConvertLog.DeserializeObject<MainModeEnterConditionData>(dataJson);

        private void Check()
        {
            float rawProgress = (float) _mainMode.GetLevelsTo(_mainMode.Id, _mainMode.Level) / _targetLevels;
            Progress = Mathf.Min(rawProgress, 1);
            if (IsConditionFulfilled()) 
                Complete();
        }

        private bool IsConditionFulfilled()
        {
            if (_mainMode.Id < _conditionData.Id) return false;
            if (_mainMode.Id > _conditionData.Id) return true;
            
            if (_mainMode.Level >= _conditionData.Level)
                return true;
            else
                return false;
        }
    }
}