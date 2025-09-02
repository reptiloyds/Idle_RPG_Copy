using System;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.View
{
    public class StuffBranchMarkHandler : BranchMarkHandler
    {
        [SerializeField, Required] private InventoryView _inventoryView;
        
        private StuffSlot _slot;
        private Action<Item> _itemEquip;
        private Action<Item> _itemRemove;

        [Inject] private BranchService _branchService;
        [Inject] private StuffInventory _inventory;

        protected override void Awake()
        {
            base.Awake();
            
            if(!ShowBranchMark) return;
            _itemEquip = OnItemEquip;
            _itemRemove = OnItemRemove;
        }

        public void SetSlot(StuffSlot slot)
        {
            if (!ShowBranchMark) return;
            _slot = slot;
            _slot.OnEquip += _itemEquip;
            _slot.OnRemove += _itemRemove;
            DrawBranchMarks();
        }

        public void ClearSlot()
        {
            if (!ShowBranchMark) return;
            _slot.OnEquip -= _itemEquip;
            _slot.OnRemove -= _itemRemove;
            ClearAllMarks();
            _slot = null;
        }

        private void OnItemEquip(Item item)
        {
            var itemView = _inventoryView.GetViewByModel(item);
            if(itemView == null) return;
            DrawBranchMark(itemView);
        }

        private void OnItemRemove(Item item)
        {
            var itemView = _inventoryView.GetViewByModel(item);
            if(itemView == null) return;
            RemoveMark(itemView.transform);
        }
        
        private void DrawBranchMarks()
        {
            foreach (var itemView in _inventoryView.Views)
            {
                if(itemView.Model == null) continue;
                if (!itemView.Model.IsEquipped) continue;
                DrawBranchMark(itemView);
            }
        }

        private void DrawBranchMark(ItemView itemView)
        {
            Branch branch = null;
            foreach (var slot in _inventory.Slots)
            {
                if(slot.Item != itemView.Model) continue;
                branch = _branchService.GetBranch(slot.BranchId);
                break;
            }
            if(branch == null) return;
            AddMark(itemView.transform, branch);
        }
    }
}