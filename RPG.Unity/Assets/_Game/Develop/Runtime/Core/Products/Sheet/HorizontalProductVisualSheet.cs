using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.PurchasePresentation;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Sheet
{
    public class HorizontalProductVisualSheet : Sheet<string, HorizontalVisualRow>
    {
        [Preserve]
        public HorizontalProductVisualSheet() { }
    }
    
        public class HorizontalVisualRow : ProductVisualRow
        {
            [Preserve] public string BackSprite { get; private set; }
            [Preserve] public PurchaseContentBackground ContentBackground { get; private set; }
            [Preserve] public string ContentColor { get; private set; }
            [Preserve] public PurchaseItemPresentType ItemPresent { get; private set; }

            private Color _contentBackgroundColor;
            public Color ContentBackgroundColor
            {
                get
                {
                    // if (_contentBackgroundColor == default) DeserializeColor(ContentColor, out _contentBackgroundColor);
                    return _contentBackgroundColor;
                }
            }

            [Preserve]
            public HorizontalVisualRow() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                DeserializeColor(ContentColor, out _contentBackgroundColor);
                if (SheetExt.IsValidationNeeded)
                    Validate();
            }
            
            protected override void Validate()
            {
                base.Validate();
                if (BackSprite != null)
                    SheetExt.CheckAsset(BackSprite);
                // DeserializeColor(ContentColor, out _contentBackgroundColor);
            }
        }
}