using System;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Skill.View
{
    public class SkillInventoryWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform _animationTarget;
        
        [SerializeField, Required] private InventoryView _inventoryView;
        [SerializeField, Required] private SkillSlotPresenter _slotPresenter;
        [SerializeField, Required] private TotalOwnedModifierView _ownedModifierView;
        [SerializeField] private WindowTweens.VerticalAnimationType _animationType = WindowTweens.VerticalAnimationType.FromBottom;
        
        [Inject] private SkillInventory _inventory;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IWindowService _windowService;
        
        public BaseButton EnhanceAllButton => _inventoryView.EnhanceButton;

        protected override void Awake()
        {
            base.Awake();
            
            _inventoryView.EnhanceAll += OnEnhanceAll;
            _inventoryView.ItemUnlock += OnItemUnlock;
            _inventoryView.ItemClick += OnItemClick;
            _inventoryView.CancelSingleItem += OnCancelSingleItem;
            _slotPresenter.OnSlotClick += OnSlotClick;
            _inventory.OnEnhanced += OnEnhance;
            _inventory.OnEquipFailed += OnEquipFailed;
            
            _inventoryView.Initialize();
            _inventoryView.ShowEquippedSign();
            
            _slotPresenter.Present();
            SetupModels();
            
            _ownedModifierView.SetViews(_inventoryView.Views);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _inventoryView.EnhanceAll -= OnEnhanceAll;
            _inventoryView.ItemUnlock -= OnItemUnlock;
            _inventoryView.ItemClick -= OnItemClick;
            _inventoryView.CancelSingleItem -= OnCancelSingleItem;
            _slotPresenter.OnSlotClick -= OnSlotClick;
            _inventory.OnEnhanced -= OnEnhance;
            _inventory.OnEquipFailed -= OnEquipFailed;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenPositionTween(_animationTarget, UseUnscaledTime, callback, _animationType);

        private void SetupModels()
        {
            var viewId = 0;
            
            foreach (var item in _inventory.Items)
            {
                _inventoryView.Setup(item, viewId, $"SkillItem_{viewId + 1}");
                viewId++;
            }

            _inventoryView.DisableEmpty();
        }

        public override void Open()
        {
            base.Open();
            
            _inventoryView.DisableSingleItem();
            _slotPresenter.DisableAccent();
            
            _slotPresenter.Redraw();
            _ownedModifierView.Redraw();
            _inventoryView.RedrawEnhanceButton();
        }

        private void OnCancelSingleItem() => 
            _slotPresenter.DisableAccent();

        private void OnEquipFailed(SkillItem item)
        {
            _inventoryView.EnableSingleItem(item);
            _slotPresenter.EnableAccent();
        }

        private async void OnSlotClick(SkillSlot slot)
        {
            if (_inventoryView.IsSingleItemEnable)
            {
                _inventory.Remove(slot.Item);
                _inventory.Equip(_inventory.GetItem(_inventoryView.SingleItem.Id));
                _inventoryView.DisableSingleItem();
                _slotPresenter.DisableAccent();
            }
            else
            {
                if (slot.BaseItem != null)
                {
                    var window = await _windowService.OpenAsync<SkillWindow>();
                    window.Setup(_inventory.Items, slot.Item);
                }   
            }
        }
        
        private async void OnItemClick(ItemView itemView)
        {
            var item = _inventory.GetItem(itemView.Model.Id);
            
            var window = await _windowService.OpenAsync<SkillWindow>();
            window.Setup(_inventory.Items, item);
        }

        private void OnItemUnlock(Item item)
        {
            _ownedModifierView.Redraw();
        }
        
        private void OnEnhance(SkillItem item)
        {
            _ownedModifierView.Redraw();
            _inventoryView.RedrawEnhanceButton();
        }

        private void OnEnhanceAll() => 
            _ownedModifierView.Redraw();
    }
}