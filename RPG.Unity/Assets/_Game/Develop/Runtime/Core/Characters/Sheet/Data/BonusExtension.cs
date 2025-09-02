using PleasantlyGames.RPG.Runtime.Core.Balance.Json;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data
{
    public static class BonusExtension
    {
        public static void DeserializeBonus(this BonusConditionType bonusConditionType, string bonusJSON)
        {
            switch (bonusConditionType)
            {
                case BonusConditionType.Level:
                    JsonConvertLog.DeserializeObject<LevelBonusData>(bonusJSON);
                    break;
                case BonusConditionType.Evolution:
                    JsonConvertLog.DeserializeObject<EvolutionBonusData>(bonusJSON);
                    break;
            }
        }
    }
}