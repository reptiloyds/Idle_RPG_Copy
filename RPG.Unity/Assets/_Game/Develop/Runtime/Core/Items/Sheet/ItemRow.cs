using Cathei.BakingSheet;
using Cathei.BakingSheet.Internal;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Sheet
{
    public class ItemRow : SheetRow<string>
    {
        [Preserve] public string LocalizationToken { get; private set; }
        [Preserve] public string ImageName { get; private set; }
        [Preserve] public ItemRarityType RarityType { get; private set; }
        [Preserve] public UnitStatType OwnedEffectType { get; private set; }
        [Preserve] public StatModType OwnedEffectModType { get; private set; }
        [Preserve] public GroupOrder OwnedEffectOrder { get; private set; }
        [Preserve] public FormulaType OwnedFormulaType { get; private set; }
        [Preserve] public string OwnedFormulaJSON { get; private set; }
        [Preserve] public int MaxLevel { get; private set; }
        [Preserve] public FormulaType UpgradeFormulaType { get; private set; }
        [Preserve] public string UpgradeFormulaJSON { get; private set; }

        [Preserve] protected ItemRow() { }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if (SheetExt.IsValidationNeeded) 
                Validate();
        }
        
        protected virtual void Validate()
        {
            if (OwnedFormulaType != FormulaType.CustomSheet) 
                OwnedFormulaType.DeserializeFormula(OwnedFormulaJSON);
            
            UpgradeFormulaType.DeserializeFormula(UpgradeFormulaJSON);

            if (MaxLevel < 0) 
                Debug.LogError("MaxLevel is less than 0");
            if (MaxLevel == 0) MaxLevel = 1;
        }
    }
}