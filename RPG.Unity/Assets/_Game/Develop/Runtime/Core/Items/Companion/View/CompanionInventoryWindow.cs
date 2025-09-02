using System;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Companion.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CompanionInventoryWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform _animationTarget;
        
        [SerializeField, Required] private InventoryView _inventoryView;
        [SerializeField, Required] private CompanionSlotPresenter _slotPresenter;
        [SerializeField, Required] private TotalOwnedModifierView _ownedModifierView;
        [SerializeField] private WindowTweens.VerticalAnimationType _animationType = WindowTweens.VerticalAnimationType.FromBottom;
        
        [Inject] private CompanionInventory _inventory;
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
            _inventory.OnEquipFailed += OnEquipFailed;
            _inventory.OnEnhanced += OnEnhance;
            
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
            _inventory.OnEquipFailed -= OnEquipFailed;
            _inventory.OnEnhanced -= OnEnhance;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenPositionTween(_animationTarget, UseUnscaledTime, callback, _animationType);
        
        private void SetupModels()
        {
            var viewId = 0;
            
            foreach (var item in _inventory.Items)
            {
                _inventoryView.Setup(item, viewId, $"CompanionItem_{viewId + 1}");
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

        private void OnCancelSingleItem()
        {
            _slotPresenter.DisableAccent();
        }

        private void OnEquipFailed(CompanionItem item)
        {
            _inventoryView.EnableSingleItem(item);
            _slotPresenter.EnableAccent();
        }

        private async void OnSlotClick(CompanionSlot slot)
        {
            if (_inventoryView.IsSingleItemEnable)
            {
                _inventory.Remove(slot.Item.Id);
                _inventory.Equip(_inventoryView.SingleItem.Id);
                _slotPresenter.DisableAccent();
                _inventoryView.DisableSingleItem();
            }
            else
            {
                if (slot.BaseItem != null)
                {
                    var window = await _windowService.OpenAsync<CompanionWindow>();
                    window.Setup(_inventory.Items, slot.Item);
                }   
            }
        }
        
        private async void OnItemClick(ItemView itemView)
        {
            var item = _inventory.GetItem(itemView.Model.Id);
            
            var window = await _windowService.OpenAsync<CompanionWindow>();
            window.Setup(_inventory.Items, item);
        }

        private void OnItemUnlock(Item item)
        {
            _ownedModifierView.Redraw();
        }
        
        private void OnEnhance(CompanionItem item)
        {
            _ownedModifierView.Redraw();
            _inventoryView.RedrawEnhanceButton();
        }

        private void OnEnhanceAll() => 
            _ownedModifierView.Redraw();
    }
}