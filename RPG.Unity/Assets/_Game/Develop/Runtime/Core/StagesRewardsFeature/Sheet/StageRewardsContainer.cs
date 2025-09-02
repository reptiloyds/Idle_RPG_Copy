using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Sheet
{
    public class StageRewardsContainer : CustomSheetContainer
    {
        [Preserve][SheetList("StageRewards")] public StageRewardsSheet stageRewards { get; private set; }

        public StageRewardsContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(stageRewards);
        }
    }
}