using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Sheet
{
    public class PeriodicRewardContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("PeriodicReward")] public PeriodicRewardSheet periodicReward { get; private set; }

        public PeriodicRewardContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            
            Balance.Set(periodicReward);
        }
    }
}