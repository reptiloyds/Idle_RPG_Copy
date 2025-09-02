using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model
{
    public class SkillItem : Item
    {
        public override ItemType Type => ItemType.Skill;
        
        public string SkillId { get; }
        public Sprite InactiveSprite { get; }

        public SkillItem(ITranslator translator, SkillItemRow stuffConfig, ItemData data, ItemConfiguration configuration, BaseValueFormula baseValueFormula,
            Sprite sprite, Sprite inactiveSprite) : base(translator, stuffConfig, data, configuration, sprite, baseValueFormula)
        {
            SkillId = stuffConfig.SkillId;
            InactiveSprite = inactiveSprite;
        }
    }
}