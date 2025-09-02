using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus.Conditions
{
    public interface IBonusCondition
    {
        public BonusConditionType Type { get; }
        public int GetMetAmount(Character character);
        string GetEnhanceCondition(Character character);
        string GetUnlockCondition(Character character);
    }
}