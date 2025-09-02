using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.Sheet
{
    public class SoftRushSheet : Sheet<int, SoftRushSheet.Row>
    {
        [Preserve] public SoftRushSheet() { }
        
        public class Row : SheetRow<int>
        {
            [Preserve] public int MaxLevel { get; private set; }
            [Preserve] public string Locations { get; private set; }
            [Preserve] public string UnitId { get; private set; }
            [Preserve] public FormulaType RewardFormulaType { get; private set; }
            [Preserve] public string RewardFormulaJSON { get; private set; }

            private List<string> _locationList;
            public List<string> LocationList => _locationList ??= JsonConvertLog.DeserializeObject<List<string>>(Locations);

            [Preserve] public Row() { }
            
            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                RewardFormulaType.DeserializeFormula(RewardFormulaJSON);
                _locationList = JsonConvertLog.DeserializeObject<List<string>>(Locations);
                foreach (var locationId in _locationList) 
                    SheetExt.CheckAsset(locationId);
                SheetExt.CheckAsset(UnitId);
            }
        }
    }
}