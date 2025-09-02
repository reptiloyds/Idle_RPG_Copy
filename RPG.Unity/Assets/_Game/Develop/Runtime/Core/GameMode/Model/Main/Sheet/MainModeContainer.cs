using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.Sheet
{
    public class MainModeContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("MainMode")] public MainModeSheet mainMode { get; private set; }

        public MainModeContainer(ILogger logger) : base(logger)
        {
        }

        public override void PostLoad()
        {
            base.PostLoad();
            if (Balance == null) return;

            Balance.Set(mainMode);
        }
    }
}