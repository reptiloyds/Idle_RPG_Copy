using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.UI.Hub.Sides;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Offers.Sheet
{
    public class ProductOffersSheet : Sheet<string, ProductOffersSheet.Row>
    {
        [Preserve]
        public ProductOffersSheet() { }
        
        public class Row : SheetRow<string>
        {
            [Preserve] public string ProductId { get; private set; }
            [Preserve] public UISideType UISideType { get; set; }
            [Preserve] public string SpriteName { get; private set; }

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
                // if(SpriteName != null)
                //     SheetExt.CheckSprite(SpriteName);
                // if (ButtonSpriteName != null)
                //     SheetExt.CheckSprite(ButtonSpriteName);
            }
        }
    }
}