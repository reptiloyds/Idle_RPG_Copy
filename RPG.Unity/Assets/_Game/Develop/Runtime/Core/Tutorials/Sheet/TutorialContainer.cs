using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet
{
    public class TutorialContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Tutorial")] public TutorialSheet tutorial { get; private set; }

        public TutorialContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(tutorial);
        }
    }
}