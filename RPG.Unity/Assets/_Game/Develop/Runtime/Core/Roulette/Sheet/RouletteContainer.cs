using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Roulette.Sheet
{
    public class RouletteContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Roulette")] public RouletteSheet roulette { get; private set; }
        
        public RouletteContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(roulette);
        }
    }
}