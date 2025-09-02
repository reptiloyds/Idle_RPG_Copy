using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Sheet;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Sheet
{
    public class BossRushContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("BossRush")] public BossRushSheet bossRush { get; private set; }
        [Preserve] [SheetList("BossRush")] public ManualStatsSheet<BossRushSheet> bossRushStats { get; private set; }
        [Preserve] [SheetList("BossRush")] public ManualFormulaSheet<BossRushSheet, string> bossRushRewards { get; private set; }

        public BossRushContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(bossRush);
            Balance.Set(bossRushStats);
            Balance.Set(bossRushRewards);
        }
    }
}