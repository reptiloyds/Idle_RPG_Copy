using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Unlock;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Config
{
    public class ImprovableUnitStatConfig : UnitStatConfig
    {
        public FormulaType PriceFormulaType;
        public string PriceFormulaJSON;
        public string ValuePostfix;
        public StatUnlockType UnlockType;
        public string UnlockJSON;
    }
}