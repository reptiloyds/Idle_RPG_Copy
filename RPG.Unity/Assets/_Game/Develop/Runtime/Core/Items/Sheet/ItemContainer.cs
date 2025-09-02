using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Sheet;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Sheet
{
    public class ItemContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Item")] public StuffItemSheet stuffItems { get; private set; }
        [Preserve] [SheetList("Item")] public CompanionItemSheet companionItems { get; private set; }
        [Preserve] [SheetList("Item")] public SkillItemSheet skillItems { get; private set; }
        
        [Preserve] [SheetList("Item")] public ManualFormulaSheet<StuffItemSheet, ItemRarityType> stuffOwnedEffects { get; private set; }
        [Preserve] [SheetList("Item")] public ManualFormulaSheet<StuffItemSheet, string> stuffEquippedEffects { get; private set; }
        [Preserve] [SheetList("Item")] public ManualFormulaSheet<CompanionItemSheet, ItemRarityType> companionOwnedEffects { get; private set; }
        [Preserve] [SheetList("Item")] public ManualFormulaSheet<SkillItemSheet, ItemRarityType> skillOwnedEffects { get; private set; }

        public ItemContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(stuffItems);
            Balance.Set(companionItems);
            Balance.Set(skillItems);
            Balance.Set(stuffOwnedEffects);
            Balance.Set(stuffEquippedEffects);
            Balance.Set(companionOwnedEffects);
            Balance.Set(skillOwnedEffects);
        }
    }
}