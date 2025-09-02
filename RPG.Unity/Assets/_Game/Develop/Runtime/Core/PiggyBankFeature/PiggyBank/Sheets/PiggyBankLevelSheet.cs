using System;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Sheets
{
    public class PiggyBankLevelSheet : Sheet<string, PiggyBankLevelSheet.Row>
    {
        [Preserve]
        public PiggyBankLevelSheet() { }
        
        public class Row : SheetRow<string>
        {
            [Preserve] public int HardLimit { get; private set; }
            [Preserve] public string SpriteId { get; private set; }
            
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
                if (HardLimit < 0)
                    Logger.LogError("HardLimit cannot be less than 0");
                
                if (String.IsNullOrEmpty(SpriteId))
                    Logger.LogError("SpriteId cannot be empty");
            }
        }
    }
}