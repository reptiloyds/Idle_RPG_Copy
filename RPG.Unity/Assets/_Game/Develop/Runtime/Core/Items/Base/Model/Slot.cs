using System;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.DebugUtilities;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.Model
{
    public abstract class Slot : IUnlockable, IDisposable
    {
        protected readonly SlotData SlotData;
        private readonly IUnlockable _unlockable;
        
        public bool IsUnlocked => _unlockable == null || _unlockable.IsUnlocked;
        public string Condition => _unlockable == null ? string.Empty : _unlockable.Condition;
        public event Action<IUnlockable> OnUnlocked;

        public Item BaseItem { get; protected set; }

        public event Action<Item> OnEquip;
        public event Action<Item> OnRemove;
        public event Action OnItemChanged;

        protected Slot(SlotData slotData, IUnlockable unlockable)
        {
            SlotData = slotData;
            _unlockable = unlockable;
            if (!IsUnlocked) 
                _unlockable.OnUnlocked += OnUnlockableUnlocked;
        }
        
        public virtual void Dispose()
        {
            if(!IsUnlocked)
                _unlockable.OnUnlocked -= OnUnlockableUnlocked;
        }
        
        private void OnUnlockableUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlockableUnlocked;
            OnUnlocked?.Invoke(this);
        }
                
        public virtual void Equip(Item item)
        {
            if (BaseItem != null)
            {
                Logger.LogError("Item already equipped");
                return;
            }
            
            BaseItem = item;
            SlotData.ItemId = item.Id;
            
            BaseItem.Equip();
            OnEquip?.Invoke(BaseItem);
            OnItemChanged?.Invoke();
        }
        
        public virtual void Remove()
        {
            SlotData.ItemId = string.Empty;

            var removedItem = BaseItem;
            BaseItem.TakeOff();
            BaseItem = null;
            OnRemove?.Invoke(removedItem);
            OnItemChanged?.Invoke();
        }
    }
}