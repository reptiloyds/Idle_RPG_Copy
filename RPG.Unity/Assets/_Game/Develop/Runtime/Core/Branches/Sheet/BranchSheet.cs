using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Branches.Sheet
{
    public class BranchSheet : Sheet<string, BranchSheet.Row>
    {
        [Preserve] public BranchSheet() { }
        
        public class Row : SheetRow<string>
        {
            [Preserve] public string CharacterId { get; private set; }
            [Preserve] public string ImageName { get; private set; }
            
            [Preserve] public Row() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);

                if (SheetExt.IsValidationNeeded) 
                    Validation();
            }

            private void Validation()
            {
                if(ImageName != null)
                    SheetExt.CheckSprite(Asset.MainAtlas, ImageName);
            }
        }
    }
}