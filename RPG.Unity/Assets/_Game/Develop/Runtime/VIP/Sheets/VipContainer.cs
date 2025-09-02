using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.VIP.Sheets
{
    public class VipContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("VIP")] public VipBonusViewSheet bonusView { get; private set; }
        
        public VipContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(bonusView);
        }
    }
}