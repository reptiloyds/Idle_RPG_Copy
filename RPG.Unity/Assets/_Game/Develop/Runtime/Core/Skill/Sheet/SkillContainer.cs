using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Sheet
{
    public class SkillContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Skill")] public SkillSheet skill { get; private set; }
        [Preserve] [SheetList("Skill")] public ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType> manualValues { get; private set; }

        public SkillContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(skill);
            Balance.Set(manualValues);
        }
    }
}