using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Items.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Sheet
{
    public class StuffItemSheet : Sheet<string, StuffItemRow>
    {
        [Preserve] public StuffItemSheet() { }
    }
    
    public class StuffItemRow : ItemRow
    {
        [Preserve] public string SlotType { get; private set; }
        [Preserve] public UnitStatType EquippedEffectType { get; private set; }
        [Preserve] public StatModType EquippedEffectModType { get; private set; }
        [Preserve] public FormulaType EquippedFormulaType { get; private set; }
        [Preserve] public string EquippedFormulaJSON { get; private set; }

        [Preserve] public StuffItemRow() { }

        protected override void Validate()
        {
            base.Validate();
            
            SheetExt.CheckSprite(Asset.StuffAtlas, ImageName);
            if (EquippedFormulaType != FormulaType.CustomSheet) 
                EquippedFormulaType.DeserializeFormula(EquippedFormulaJSON);
            ValidateSlotTag();
        }

        private void ValidateSlotTag()
        {
            if (SlotType.IsNullOrWhitespace())
            {
                Debug.LogError("SlotTypeTag is null or whitespace");
                return;
            }
            SheetExt.CheckAsset(SlotType);
        }
    }
}