using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.VIP.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.VIP.Sheets
{
    public class VipBonusViewSheet : Sheet<VipBonusType, VipBonusViewSheet.Row>
    {
        [Preserve]
        public VipBonusViewSheet()
        {
        }

        public class Row : SheetRow<VipBonusType>
        {
            [Preserve] public string Sprite { get; private set; }
            [Preserve] public string LabelToken { get; private set; }
            [Preserve] public string DefinitionToken { get; private set; }

            [Preserve]
            public Row()
            {
            }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);

                if (SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                if (string.IsNullOrEmpty(Sprite)) 
                    Logger.LogError($"Sprite can`t be null");
                if (string.IsNullOrEmpty(LabelToken)) 
                    Logger.LogError($"LabelToken can`t be null");
                if (string.IsNullOrEmpty(DefinitionToken)) 
                    Logger.LogError($"DefinitionToken can`t be null");
            }
        }
    }
}