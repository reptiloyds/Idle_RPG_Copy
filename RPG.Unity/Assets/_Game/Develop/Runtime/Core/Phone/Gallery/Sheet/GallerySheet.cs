using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Sheet
{
    public class GallerySheet : Sheet<string, GallerySheet.Row>
    {
        [Preserve]
        public GallerySheet() { }
        
        public class Row : SheetRow<string>
        {
            [Preserve] public string SpriteName { get; private set; }
            
            [Preserve]
            public Row() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                SheetExt.CheckAsset(SpriteName + Photo.LowQualityPostfix);
                SheetExt.CheckAsset(SpriteName + Photo.HighQualityPostfix);
            }
        }
    }
}