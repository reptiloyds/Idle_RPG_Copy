using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Sheets
{
    public class PiggyBankConstantsSheet : Sheet<string, PiggyBankConstantsSheet.Row>
    {
        [Preserve]
        public PiggyBankConstantsSheet() { }
        
        public class Row : SheetRow<string>
        {
            [Preserve] public int Rg { get; private set; }
            [Preserve] public int Kl { get; private set; }
            
            [Preserve]
            public Row() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                if (Rg <= 0)
                    Logger.LogError("Rg must be greater than 0");

                if (Kl <= 0)
                    Logger.LogError("Kl must be greater than 0");
            }
        }
    }
}