using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.Sheets
{
    public class ResourceSheet : Sheet<ResourceType, ResourceSheet.Row>
    {
        [Preserve] public ResourceSheet() { }
        
        public class Row : SheetRow<ResourceType>
        {
            [Preserve] public int Value { get; private set; }
            [Preserve] public string ImageName { get; private set; }
            [Preserve] public bool InvisibleWhenZero { get; private set; }

            [Preserve] public Row() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if (SheetExt.IsValidationNeeded) 
                    Validate();
            }

            private void Validate() => 
                SheetExt.CheckSprite(Asset.MainAtlas, ImageName);
        }
    }
}