using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Stats.Extension;
using PleasantlyGames.RPG.Runtime.Core.Stats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Sheet
{
    public class BossRushSheet : Sheet<int, BossRushSheet.Row>
    {
        [Preserve] public BossRushSheet() { }
        
        public class Row : SheetRow<int>
        {
            [Preserve] public int MaxLevel { get; private set; }
            [Preserve] public string Locations { get; private set; }
            [Preserve] public int UnitAmount { get; private set; }
            [Preserve] public string UnitIds { get; private set; }
            [Preserve] public FormulaType RewardFormulaType { get; private set; }
            [Preserve] public string RewardFormulaJSON { get; private set; }

            private List<string> _locationList;
            public List<string> LocationList => _locationList ??= JsonConvertLog.DeserializeObject<List<string>>(Locations);

            private List<string> _units;
            public List<string> Units => _units ??= DeserializeUnits();

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
                
                _units = DeserializeUnits();

                foreach (var unit in _units) 
                    SheetExt.CheckAsset(unit);
            }

            private List<string> DeserializeUnits()
            {
                if (string.IsNullOrWhiteSpace(UnitIds))
                {
                    Debug.LogError("BossesJSON is null or whitespace");
                    return null;
                }

                return JsonConvertLog.DeserializeObject<List<string>>(UnitIds);
            }
        }
    }
}