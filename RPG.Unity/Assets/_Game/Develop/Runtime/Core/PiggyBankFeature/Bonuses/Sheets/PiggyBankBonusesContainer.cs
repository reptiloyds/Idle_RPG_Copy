using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Sheets
{
    public class PiggyBankBonusesContainer : CustomSheetContainer
    {
        [Preserve][SheetList("PiggyBank")] public PiggyBankBonusesSheet piggyBankBonuses { get; private set; }
        
        [Preserve]
        public PiggyBankBonusesContainer(ILogger logger) : base(logger)
        {
        }
        
        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            
            Balance.Set(piggyBankBonuses);
        }
    }
}