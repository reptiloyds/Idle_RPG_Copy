using System;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Cheats;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class StuffInventoryWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField, Required] private InventoryView _inventoryView;
        [SerializeField, Required] private SelectedStuffView _selectedStuffView;
        [SerializeField, Required] private TotalOwnedModifierView _ownedModifierView;
        [SerializeField, Required] private TextMeshProUGUI _slotName;
        [SerializeField, Required] private BaseButton _cheatButton;
        [SerializeField, Required] private StuffBranchMarkHandler _branchMarkHandler; 
        
        private StuffSlot _slot;

        [Inject] private BranchService _branchService;
        [Inject] private StuffInventory _inventory;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private ITranslator _translator;
        [Inject] private CheatService _cheatService;

        public StuffSlot Slot => _slot;
        public StuffItem SelectedItem => _selectedStuffView.Item;
        public BaseButton EquipButton => _selectedStuffView.EquipButton;
        public BaseButton EnhanceAllButton => _inventoryView.EnhanceButton;

        public event Action OnSetup;
        public event Action OnItemSelected;

        protected override void Awake()
        {
            base.Awake();
            
            _translator.OnChangeLanguage += OnChangeLanguage;
            _inventoryView.EnhanceAll += OnEnhanceAll;
            _inventoryView.ItemUnlock += OnItemUnlock;
            _inventoryView.ItemClick += OnItemClick;
            
            _selectedStuffView.OnEquip += EquipItem;
            _selectedStuffView.OnEnhance += EnhanceItem;
            
            _selectedStuffView.Initialize();
            _inventoryView.Initialize();
            _inventoryView.HideEquippedSign();
            
            _ownedModifierView.SetViews(_inventoryView.Views);
            
            _cheatButton.gameObject.SetActive(_cheatService.IsEnabled);
            if (_cheatService.IsEnabled) 
                _cheatButton.OnClick += OnCheatClick;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _translator.OnChangeLanguage -= OnChangeLanguage;
            _inventoryView.EnhanceAll -= OnEnhanceAll;
            _inventoryView.ItemUnlock -= OnItemUnlock;
            _inventoryView.ItemClick -= OnItemClick;
            _selectedStuffView.OnEquip -= EquipItem;
            _selectedStuffView.OnEnhance -= EnhanceItem;
            
            if (_cheatService.IsEnabled) 
                _cheatButton.OnClick -= OnCheatClick;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);
        
        private void OnCheatClick() => 
            _cheatService.AppendItem(_selectedStuffView.Item.Id);

        public void Setup(StuffSlot slot)
        {
            if(_slot != null)
                ClearSlot();
            _slot = slot;
            _slotName.SetText(_translator.Translate(_slot.Type.LocalizationToken));
            if (!SetupModels())
            {
                Close(); 
                return;   
            }
            _ownedModifierView.Redraw();
            _inventoryView.RedrawEnhanceButton();
            _inventoryView.DisableSingleItem();
            
            _branchMarkHandler.SetSlot(_slot);
            
            OnSetup?.Invoke();
        }

        public override void Close()
        {
            base.Close();
            _inventoryView.Clear();
        }

        private void ClearSlot()
        {
            if(_slot == null) return;
            _branchMarkHandler.ClearSlot();
        }

        public void SetEquippedItem(StuffItem item)
        {
            if (item != null)
            {
                SelectItem(item);
                _selectedStuffView.SetEquippedItem(item);
                _selectedStuffView.Redraw();
            }
            else
            {
                _selectedStuffView.SetEquippedItem(null);
                var stuffItem = _inventory.GetItem(_inventoryView.Views.First().Model.Id);
                SelectItem(stuffItem);
            }
        }

        public ItemView GetViewByModel(Item item) => 
            _inventoryView.GetViewByModel(item);

        private bool SetupModels()
        {
            var viewId = 0;
            
            foreach (var item in _inventory.Items)
            {
                if(item.SlotType != _slot.Type) continue;
                _inventoryView.Setup(item, viewId, $"StuffItem_{viewId + 1}");
                viewId++;
            }
            
            _inventoryView.SortByRarity();

            _inventoryView.DisableEmpty();

            return viewId > 0;
        }

        private void SelectItem(StuffItem item)
        {
            _inventoryView.Select(item);
            _selectedStuffView.Draw(item);
            OnItemSelected?.Invoke();
        }

        private void EquipItem()
        {
            var selectedStuffItem = _inventory.GetItem(_inventoryView.SelectedItem.Id);
            _inventory.Equip(selectedStuffItem, _slot);
            SetEquippedItem(selectedStuffItem);
        }

        private void EnhanceItem()
        {
            _inventory.Enhance(_inventoryView.SelectedItem.Id);
            _ownedModifierView.Redraw();
            _inventoryView.RedrawEnhanceButton();
        }

        private void OnItemUnlock(Item item) =>
            _ownedModifierView.Redraw();
        
        private void OnChangeLanguage()
        {
            if(!IsOpened) return;
            
            if (_slot != null) _slotName.SetText(_translator.Translate(_slot.Type.LocalizationToken));
            _ownedModifierView.Redraw();
        }

        private void OnEnhanceAll() => 
            _ownedModifierView.Redraw();

        private void OnItemClick(ItemView itemView)
        {
            var stuffItem = _inventory.GetItem(itemView.Model.Id);
            SelectItem(stuffItem);
        }
    }
}