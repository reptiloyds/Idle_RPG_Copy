using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet
{
    public class BlessingSheetsContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Blessing")] public BlessingSheet blessing { get; private set; }
        [Preserve] [SheetList("Blessing")] public GlobalBlessingSheet globalBlessing { get; private set; }

        public BlessingSheetsContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            
            Balance.Set(blessing);
            Balance.Set(globalBlessing);
        }
    }
}