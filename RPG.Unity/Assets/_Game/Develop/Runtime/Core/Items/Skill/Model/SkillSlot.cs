using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Save;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model
{
    public class SkillSlot : Slot       
    {
        private readonly SkillData _data;
                
        public int Id => _data.SlotId;
        public SkillItem Item { get; private set; }
        
        public SkillSlot(SkillData data, IUnlockable unlockable) : base(data, unlockable) => 
            _data = data;

        public void Equip(SkillItem item)
        {
            Item = item;
            base.Equip(item);
        }
        
        public override void Remove()
        {
            Item = null;
            base.Remove();
        }
    }
}