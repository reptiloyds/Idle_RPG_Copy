using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus.Conditions
{
    public class EvolutionCondition : IBonusCondition
    {
        private readonly ITranslator _translator;
        private readonly EvolutionBonusData _data;

        public BonusConditionType Type => BonusConditionType.Evolution;

        public EvolutionCondition(ITranslator translator, string dataJSON)
        {
            _translator = translator;
            _data = JsonConvertLog.DeserializeObject<EvolutionBonusData>(dataJSON);
        }

        public int GetMetAmount(Character character)
        {
            var counter = 0;
            foreach (var evolution in _data.Evolutions)
                if (evolution <= character.Evolution)
                    counter++;

            return counter;
        }

        public string GetUnlockCondition(Character character) => 
            GetRequiredEvolution(character.Evolution).ToString();

        public string GetEnhanceCondition(Character character)
        {
            int requiredEvolution = GetRequiredEvolution(character.Evolution);
            if (requiredEvolution == 0) return string.Empty;
            return string.Format(_translator.Translate(TranslationConst.BonusEvolutionCondition), requiredEvolution);
        }

        private int GetRequiredEvolution(int characterEvolution)
        {
            foreach (var evolution in _data.Evolutions)
            {
                if(evolution <= characterEvolution) continue;
                return evolution;
            }

            return 0;
        }
    }
}