using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Save;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model
{
    public class CompanionSlot : Slot
    {
        private readonly CompanionData _data;
        
        public int Id => _data.SlotId;
        public CompanionItem Item { get; private set; }
            
        public CompanionSlot(CompanionData data, IUnlockable unlockable) : base(data, unlockable)
        {
            _data = data;
        }

        public void Equip(CompanionItem item)
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