using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine.Scripting;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Sheet
{
    public class UnitContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Unit")] public PlayerStatsSheet playerStats { get; private set; }
        [Preserve] [SheetList("Unit")] public UnitsSheet units { get; private set; }
        [Preserve] [SheetList("Unit")] public ExtendedManualFormulaSheet<UnitsSheet, string, UnitStatType> manualStats { get; private set; }

        public UnitContainer(ILogger logger) : base(logger)
        {
        }
        
        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            
            Balance.Set(units);
            Balance.Set(playerStats);
            Balance.Set(manualStats);
        }
    }
}