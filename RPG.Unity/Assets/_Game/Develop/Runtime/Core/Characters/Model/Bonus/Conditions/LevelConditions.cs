using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus.Conditions
{
    internal class LevelCondition : IBonusCondition
    {
        private readonly LevelBonusData _data;
        private readonly ITranslator _translator;
        
        public LevelCondition(ITranslator translator, string dataJSON)
        {
            _translator = translator;
            _data = JsonConvertLog.DeserializeObject<LevelBonusData>(dataJSON);
        }

        public BonusConditionType Type => BonusConditionType.Level;

        public int GetMetAmount(Character character)
        {
            var counter = 0;
            foreach (var level in _data.Levels)
                if(level <= character.Level)
                    counter++;

            return counter;
        }

        public string GetUnlockCondition(Character character)
        {
            int requiredLevel = GetRequiredLevel(character.Level);
            return $"{TranslationConst.LevelPrefixCaps} {requiredLevel}";
        }

        public string GetEnhanceCondition(Character character)
        {
            int requiredLevel = GetRequiredLevel(character.Level);
            
            if (_data.Levels.Length >= 2 && _data.Levels[0] == requiredLevel) 
                requiredLevel = _data.Levels[1];
            
            if (requiredLevel == 0) return string.Empty;
            return string.Format(_translator.Translate(TranslationConst.BonusLevelCondition), requiredLevel);
        }

        private int GetRequiredLevel(int characterLevel)
        {
            foreach (var level in _data.Levels)
            {
                if(level <= characterLevel) continue;
                return level;
            }
            
            return 0;
        }
    }
}