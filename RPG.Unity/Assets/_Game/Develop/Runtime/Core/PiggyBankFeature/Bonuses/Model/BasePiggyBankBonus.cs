using System;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Data;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Sheets;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Model
{
    public abstract class BasePiggyBankBonus
    {
        private PiggyBankBonusesSheet.PiggyBankBonusRow _config;
        private PiggyBankBonusData _data;
        private ITranslator _translator;
        
        private const string NeedLevelToken = "piggy_bank_bonus_need_level";

        public bool IsCollected => _data.IsCollected;
        public int LevelNeed => _config.LevelNeed;

        protected BasePiggyBankBonus(
            PiggyBankBonusData data,
            PiggyBankBonusesSheet.PiggyBankBonusRow config, 
            ITranslator translator)
        {
            _data = data;
            _config = config;
            _translator = translator;
        }
        
        public abstract Sprite GetIcon();
        public abstract int GetAmount();

        public virtual void Collect(Vector3 vfxPosition, float vfxDelay)
        {
            _data.IsCollected = true;
        }

        public string GetNeedLevelText()
        {
            return $"{_translator.Translate(NeedLevelToken)}{_config.LevelNeed}";
        }
    }
}