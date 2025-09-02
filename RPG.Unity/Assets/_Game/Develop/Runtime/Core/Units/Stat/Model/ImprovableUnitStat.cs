using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Config;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Save;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Unlock;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model
{
    public class ImprovableUnitStat : UnitStat
    {
        private readonly ImprovableUnitStatConfig _improveConfig;
        private readonly BaseValueFormula _priceFormula;
        
        public string ValuePostfix => _improveConfig.ValuePostfix;
        public StatUnlockType UnlockType => _improveConfig.UnlockType;
        public string UnlockJSON => _improveConfig.UnlockJSON;

        public ImprovableUnitStat(StatData statData, ImprovableUnitStatConfig config, Sprite sprite) : base(statData, config, sprite)
        {
            _improveConfig = config; 
            _priceFormula = _improveConfig.PriceFormulaType.CreateFormula(_improveConfig.PriceFormulaJSON);
        }
        
        public BigDouble.Runtime.BigDouble GetPrice() => 
            BigDouble.Runtime.BigDouble.Ceiling(_priceFormula.CalculateBigDouble(StatData.Level));
    }
}