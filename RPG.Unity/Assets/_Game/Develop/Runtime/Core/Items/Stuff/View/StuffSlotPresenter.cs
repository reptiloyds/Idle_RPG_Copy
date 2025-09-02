using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class StuffSlotPresenter : MonoBehaviour
    {
        [SerializeField] private List<StuffSlotView> _slotViews;

        private string _branchId;

        [Inject] private BranchService _branchService;
        [Inject] private StuffInventory _inventory;

        public event Action<StuffSlot> OnSlotClick;

        private void Awake()
        {
            foreach (var slotView in _slotViews) 
                slotView.SuccessClick += OnSuccessSlotClick;
        }

        private void OnDestroy()
        {
            foreach (var slotView in _slotViews) 
                slotView.SuccessClick -= OnSuccessSlotClick;
        }

        private void OnSuccessSlotClick(SlotView<StuffSlot> slotView) => 
            OnSlotClick?.Invoke(slotView.Slot);

        public void Present()
        {
            var branch = _branchService.GetSelectedBranch();
            if(_branchId == branch.Id) return;
            
            _branchId = branch.Id;
            int viewId = 0;
            foreach (var slot in _inventory.Slots)
            {
                if(!string.Equals(slot.BranchId, branch.Id)) continue;
                
                _slotViews[viewId].SetModel(slot);
                _slotViews[viewId].Enable();
                viewId++;
                
                if (viewId < _slotViews.Count) continue;
                Logger.LogError("Too many slot models");
                break;
            }

            for (; viewId < _slotViews.Count; viewId++) 
                _slotViews[viewId].Disable();
        }
        
        public StuffSlotView GetViewByModel(StuffSlot slot)
        {
            foreach (var view in _slotViews)
                if (view.Slot == slot) return view;

            return null;
        }

        public void Redraw()
        {
            foreach (var slotView in _slotViews) 
                slotView.Redraw();
        }

        [Button]
        private void UpdateComponents() => 
            _slotViews = GetComponentsInChildren<StuffSlotView>().ToList();
    }
}