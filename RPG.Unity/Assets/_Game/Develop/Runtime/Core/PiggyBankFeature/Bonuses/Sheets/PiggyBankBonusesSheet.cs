using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Extension;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Sheets
{
    public class PiggyBankBonusesSheet : Sheet<string, PiggyBankBonusesSheet.PiggyBankBonusRow>
    {
        [Preserve]
        public PiggyBankBonusesSheet() { }
        
        public class PiggyBankBonusRow : SheetRow<string>
        {
            [Preserve] public int LevelNeed { get; private set; }
            [Preserve] public PiggyBankRewardType RewardType { get; private set; }
            [Preserve] public string ItemJson { get; private set; }
            
            [Preserve]
            public PiggyBankBonusRow() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                RewardType.TryDeserialize(ItemJson);
            }
        }
    }
}