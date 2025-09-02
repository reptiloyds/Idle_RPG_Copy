using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet
{
    public class DailiesContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Dailies")] public DailiesSheet quests { get; private set; }
        
        public DailiesContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(quests);
        }
    }
}