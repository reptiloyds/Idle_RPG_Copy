using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Sheets
{
    public class PiggyBankContainer : CustomSheetContainer
    {
        [Preserve][SheetList("PiggyBank")] public PiggyBankConstantsSheet piggyBankConst { get; private set; }
        [Preserve][SheetList("PiggyBank")] public PiggyBankLevelSheet piggyBankLevels { get; private set; }
        
        [Preserve]
        public PiggyBankContainer(ILogger logger) : base(logger)
        {
        }
        
        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            
            Balance.Set(piggyBankConst);
            Balance.Set(piggyBankLevels);
        }
    }
}