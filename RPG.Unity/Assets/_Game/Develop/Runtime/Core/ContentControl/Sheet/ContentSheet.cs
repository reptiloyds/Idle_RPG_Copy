using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Extension;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Sheet
{
    public class ContentSheet : Sheet<string, ContentSheet.Row>
    {
        [Preserve] public ContentSheet() { }
        
        public class Row : SheetRow<string>
        {
            [Preserve] public ContentType ContentType { get; private set; }
            [Preserve] public string ContentDataJSON { get; private set; }
            [Preserve] public UnlockType UnlockType { get; private set; }
            [Preserve] public string UnlockDataJSON { get; private set; }
            [Preserve] public string Dependency { get; private set; }
            [Preserve] public bool ManualUnlock { get; private set; }
            [Preserve] public string LocalizationToken { get; private set; }
            [Preserve] public string ImageName { get; private set; }
            
            [Preserve] public Row() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);

                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                ContentType.DeserializeData(ContentDataJSON);
                UnlockType.DeserializeData(UnlockDataJSON);
            }
        }
    }
}