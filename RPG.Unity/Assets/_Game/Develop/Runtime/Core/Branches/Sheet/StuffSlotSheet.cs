using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Branches.Sheet
{
    public class StuffSlotSheet : Sheet<string, StuffSlotSheet.Row>
    {
        [Preserve] public StuffSlotSheet() { }
        
        public class Row : SheetRow<string>
        {
            [Preserve] public string BranchId { get; private set; }
            [Preserve] public string Type { get; private set; }
            
            [Preserve]
            public Row() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);

                if (SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                SheetExt.CheckAsset(Type);
                if (string.IsNullOrEmpty(BranchId)) 
                    Logger.LogError("BranchId cannot be empty");
            }
        }
    }
}