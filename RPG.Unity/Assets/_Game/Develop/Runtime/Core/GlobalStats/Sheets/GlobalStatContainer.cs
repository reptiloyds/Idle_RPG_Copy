using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GlobalStats.Sheets
{
    public class GlobalStatContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("GlobalStats")] public GlobalStatSheet globalStats { get; private set; }

        public GlobalStatContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            
            Balance.Set(globalStats);
        }
    }
}