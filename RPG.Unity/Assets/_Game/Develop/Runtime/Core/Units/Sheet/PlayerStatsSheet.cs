using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Config;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Unlock;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Sheet
{
    public class PlayerStatsSheet : Sheet<UnitStatType, PlayerStatsSheet.Row>
    {
        [Preserve] public PlayerStatsSheet() { }
        
        public class Row : SheetRow<UnitStatType>
        {
            [Preserve] public UnitStatType StatType { get; private set; }
            [Preserve] public FormulaType ValueFormulaType { get; private set; }
            [Preserve] public string ValueFormulaJSON { get; private set; }
            [Preserve] public int MaxLevel { get; private set; }
            [Preserve] public string ImageName { get; private set; }
            
            [Preserve] public FormulaType PriceFormulaType { get; private set; }
            [Preserve] public string PriceFormulaJSON { get; private set; }
            [Preserve] public string ValuePostfix { get; set; }
            [Preserve] public StatUnlockType UnlockType { get; private set; }
            [Preserve] public string UnlockJSON { get; private set; }
            
            public ImprovableUnitStatConfig Config { get; private set; }
            
            [Preserve]
            public Row()
            {
            }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);

                Config = new ImprovableUnitStatConfig()
                {
                    StatType = Id,
                    ValueFormulaType = ValueFormulaType,
                    ValueFormulaJSON = ValueFormulaJSON,
                    MaxLevel = MaxLevel,
                    ImageName = ImageName,
                    
                    PriceFormulaType = PriceFormulaType,
                    PriceFormulaJSON = PriceFormulaJSON,
                    ValuePostfix = ValuePostfix,
                    UnlockType = UnlockType,
                    UnlockJSON = UnlockJSON,
                };

                if (SheetExt.IsValidationNeeded) 
                    Validate();
            }

            private void Validate()
            {
                PriceFormulaType.DeserializeFormula(PriceFormulaJSON);
                
                switch (UnlockType)
                {
                    case StatUnlockType.StatLevel:
                        JsonConvertLog.DeserializeObject<StatLevelUnlockData>(UnlockJSON);
                        break;
                }
                
                ValueFormulaType.DeserializeFormula(ValueFormulaJSON);

                if (MaxLevel < 0) 
                    Debug.LogError("MaxLevel is less than 0");
                if (MaxLevel == 0) MaxLevel = 1;
                
                if(!string.IsNullOrEmpty(ImageName))
                    SheetExt.CheckSprite(Asset.MainAtlas, ImageName);
            }
        }
    }
}