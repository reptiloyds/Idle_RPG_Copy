using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Items.Sheet;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Skill.Sheet
{
    public class SkillItemSheet : Sheet<string, SkillItemRow>
    {
        [Preserve] public SkillItemSheet() { }
    }
    
    public class SkillItemRow : ItemRow
    {
        [Preserve] public string SkillId { get; private set; }
        
        [Preserve] public SkillItemRow() { }

        protected override void Validate()
        {
            base.Validate();
            
            SheetExt.CheckSprite(Asset.SkillAtlas, ImageName);
        }
    }
}