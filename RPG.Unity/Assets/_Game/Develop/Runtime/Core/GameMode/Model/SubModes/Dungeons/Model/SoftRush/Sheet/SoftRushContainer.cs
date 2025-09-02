using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Sheet;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.Sheet
{
    public class SoftRushContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("SoftRush")] public SoftRushSheet softRush { get; private set; }
        [Preserve] [SheetList("SoftRush")] public ManualStatsSheet<SoftRushSheet> softRushStats { get; private set; }
        [Preserve] [SheetList("SoftRush")] public ManualFormulaSheet<SoftRushSheet, string> softRushRewards { get; private set; }

        public SoftRushContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(softRush);
            Balance.Set(softRushStats);
            Balance.Set(softRushRewards);
        }
    }
}