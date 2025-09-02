using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Skill.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SkillSlotPresenter : MonoBehaviour
    {
        [SerializeField] private List<SkillSlotView> _slotViews;
        
        [Inject] private SkillInventory _inventory;
        
        public event Action<SkillSlot> OnSlotClick;

        private void Awake()
        {
            foreach (var slotView in _slotViews) 
                slotView.SuccessClick += OnSlotViewClick;
        }

        private void OnDestroy()
        {
            foreach (var slotView in _slotViews) 
                slotView.SuccessClick -= OnSlotViewClick;
        }

        public void Present()
        {
            int viewId = 0;
            foreach (var slot in _inventory.Slots)
            {
                if (viewId >= _slotViews.Count)
                {
                    Logger.LogError("Too many slot models");
                    break;   
                }
                _slotViews[viewId].SetModel(slot);
                _slotViews[viewId].Enable();
                viewId++;
            }

            for (; viewId < _slotViews.Count; viewId++) 
                _slotViews[viewId].Disable();

            for (var i = 0; i < _slotViews.Count; i++)
            {
                var slot = _slotViews[i];
                slot.Button.ChangeButtonId($"SkillSlot_{i + 1}");
            }
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

        private void OnSlotViewClick(SlotView<SkillSlot> slot) => 
            OnSlotClick?.Invoke(slot.Slot);

        [Button]
        private void UpdateComponents() => 
            _slotViews = GetComponentsInChildren<SkillSlotView>().ToList();
    }
}