using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Companion.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CompanionSlotPresenter : MonoBehaviour
    {
        [SerializeField] private List<CompanionSlotView> _slotViews;
        [Inject] private CompanionInventory _inventory;
        
        public event Action<CompanionSlot> OnSlotClick;

        private void Awake()
        {
            for (int i = 0; i < _slotViews.Count; i++)
            {
                var slotView = _slotViews[i];
                slotView.Button.ChangeButtonId($"CompanionSlot_{i + 1}");
                slotView.SuccessClick += OnSlotViewClick;   
            }
        }

        private void OnDestroy()
        {
            foreach (var slotView in _slotViews) 
                slotView.SuccessClick -= OnSlotViewClick;
        }

        public void Present()
        {
            int viewId = 0;
            foreach (var slotModel in _inventory.Slots)
            {
                if (viewId >= _slotViews.Count)
                {
                    Logger.LogError("Too many slot models");
                    break;   
                }
                _slotViews[viewId].SetModel(slotModel);
                _slotViews[viewId].Enable();
                viewId++;
            }

            for (; viewId < _slotViews.Count; viewId++) 
                _slotViews[viewId].Disable();
        }

        public void EnableAccent()
        {
            foreach (var view in _slotViews)
            {
                if(!view.gameObject.activeSelf) continue;
                view.EnableAccent();  
            } 
        }

        public void DisableAccent()
        {
            foreach (var view in _slotViews)
            {
                if(!view.gameObject.activeSelf) continue;
                view.DisableAccent();  
            } 
        }

        public void Redraw()
        {
            foreach (var slotView in _slotViews) 
                slotView.Redraw();
        }

        private void OnSlotViewClick(SlotView<CompanionSlot> view) => 
            OnSlotClick?.Invoke(view.Slot);

        [Button]
        private void UpdateComponents() => 
            _slotViews = GetComponentsInChildren<CompanionSlotView>().ToList();
    }
}