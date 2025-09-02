using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Config
{
    public class UnitStatConfig
    {
        public UnitStatType StatType;
        public FormulaType ValueFormulaType;
        public string ValueFormulaJSON;
        public int MaxLevel;
        public string ImageName;
    }
}