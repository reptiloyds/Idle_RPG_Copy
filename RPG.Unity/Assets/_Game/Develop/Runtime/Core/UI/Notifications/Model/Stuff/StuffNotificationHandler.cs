using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Core.Units.UI;
using PleasantlyGames.RPG.Runtime.Pool;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Stuff
{
    public class StuffNotificationHandler
    {
        [Inject] private StuffInventory _inventory;
        [Inject] private IWindowService _windowService;
        [Inject] private NotificationConfiguration _configuration;
        [Inject] private BranchService _branchService;

        private IReadOnlyList<StuffItem> _items;

        private StuffItem _effectiveItem;

        private Notification _itemNotification;
        private Notification _equipNotification;
        private Notification _enhanceNotification;

        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private readonly StuffSlot _slot;

        private readonly string _inventoryWindowType = nameof(StuffInventoryWindow);
        private StuffInventoryWindow _inventoryWindow;
        private readonly string _branchWindowType = nameof(BranchWindow);
        private BranchWindow _branchWindow;

        public Notification SlotNotification { get; private set; }

        private bool IsInventoryWithCurrentSlot =>
            _inventoryWindow != null && _inventoryWindow.IsOpened && _inventoryWindow.Slot == _slot;

        public StuffNotificationHandler(ObjectPoolWithParent<NotificationView> pool, StuffSlot slot)
        {
            _pool = pool;
            _slot = slot;
        }

        public void Initialize()
        {
            _items = _inventory.Items.Where(x => Equals(x.SlotType, _slot.Type)).ToList();
            _itemNotification = new Notification(_pool, _configuration.StuffSetup.ItemSetup,
                _configuration.StuffSetup.CommonImageSetup);
            _equipNotification = new Notification(_pool, _configuration.StuffSetup.EquipSetup,
                _configuration.StuffSetup.CommonImageSetup);
            _enhanceNotification = new Notification(_pool, _configuration.StuffSetup.EnhanceSetup,
                _configuration.StuffSetup.CommonImageSetup);

            SlotNotification = new Notification(_pool, _configuration.StuffSetup.SlotSetup);
            SlotNotification.AppendChild(_itemNotification);
            SlotNotification.AppendChild(_enhanceNotification);
            SlotNotification.AppendChild(_equipNotification);
        }

        public void Enable()
        {
            var stuffWindowExist = _windowService.IsExist<StuffInventoryWindow>();
            var branchWindowExist = _windowService.IsExist<BranchWindow>();
            if (!stuffWindowExist || !branchWindowExist)
                _windowService.OnCreate += OnWindowCreate;

            if (stuffWindowExist)
                HandleInventoryWindowAsync().Forget();

            if (branchWindowExist)
                HandleBranchWindowAsync().Forget();

            _inventory.OnEquip += OnEquip;

            foreach (var item in _items)
            {
                item.OnLevelUp += OnItemLevelUp;
                item.OnUnlock += OnItemUnlock;
                item.OnAmountChanged += OnItemAmountChanged;
            }

            CheckItem();
            CheckEquip();
            CheckEnhance();
        }

        public void Disable()
        {
            _inventory.OnEquip -= OnEquip;

            foreach (var item in _items)
            {
                item.OnLevelUp -= OnItemLevelUp;
                item.OnUnlock -= OnItemUnlock;
                item.OnAmountChanged -= OnItemAmountChanged;
            }

            var stuffWindowExist = _windowService.IsExist<StuffInventoryWindow>();
            var branchWindowExist = _windowService.IsExist<BranchWindow>();

            if (!stuffWindowExist || !branchWindowExist)
                _windowService.OnCreate -= OnWindowCreate;

            if (stuffWindowExist)
                UnsubscribeInventoryWindow();

            if (branchWindowExist)
                UnsubscribeBranchWindow();

            _itemNotification.RemoveParent();
            _itemNotification.Disable();
            SlotNotification.RemoveParent();
            //_slotNotification.Disable();
            _equipNotification.RemoveParent();
            _equipNotification.Disable();
            _enhanceNotification.RemoveParent();
            _enhanceNotification.Disable();
        }

        private async void OnWindowCreate(string windowType)
        {
            if (string.Equals(windowType, _inventoryWindowType))
                await HandleInventoryWindowAsync();
            else if (string.Equals(windowType, _branchWindowType))
                await HandleBranchWindowAsync();
            else
                return;

            if (_inventoryWindow != null && _branchWindow != null)
                _windowService.OnCreate -= OnWindowCreate;
        }

        private async UniTask HandleInventoryWindowAsync()
        {
            _inventoryWindow = await _windowService.GetAsync<StuffInventoryWindow>(false);
            if (_inventoryWindow.IsOpened)
            {
                SetupSlotParent();
                CheckItem();
                CheckEquip();
                CheckEnhance();
            }
            SubscribeInventoryWindow();
        }

        private async UniTask HandleBranchWindowAsync()
        {
            _branchWindow ??= await _windowService.GetAsync<BranchWindow>(false);
            if (_branchWindow.IsOpened)
                SetupSlotParent();
            SubscribeBranchWindow();
        }

        private void SubscribeBranchWindow()
        {
            _branchWindow.OnOpen += OnBranchWindowOpen;
            _branchWindow.OnClosed += OnBranchWindowClose;
        }

        private void UnsubscribeBranchWindow()
        {
            _branchWindow.OnOpen -= OnBranchWindowOpen;
            _branchWindow.OnClosed -= OnBranchWindowClose;
        }

        private void SubscribeInventoryWindow()
        {
            _inventoryWindow.OnSetup += OnInventoryWindowSetup;
            _inventoryWindow.OnClosed += OnInventoryWindowClosed;
            _inventoryWindow.OnItemSelected += CheckEquip;
        }

        private void UnsubscribeInventoryWindow()
        {
            _inventoryWindow.OnSetup -= OnInventoryWindowSetup;
            _inventoryWindow.OnClosed -= OnInventoryWindowClosed;
            _inventoryWindow.OnItemSelected -= CheckEquip;
        }

        private void OnBranchWindowOpen(BaseWindow window) =>
            SetupSlotParent();

        private void SetupSlotParent()
        {
            var slotView = _branchWindow.GetViewByModel(_slot);
            if (slotView == null) return;
            SlotNotification.SetParent(slotView.transform);
        }

        private void OnBranchWindowClose(BaseWindow window) =>
            SlotNotification.RemoveParent();

        private void OnInventoryWindowSetup()
        {
            if (!IsInventoryWithCurrentSlot) return;

            SetupItemParents();
            CheckEquip();
        }

        private void SetupItemParents()
        {
            if (_effectiveItem != null)
                _itemNotification.SetParent(_inventoryWindow.GetViewByModel(_effectiveItem).transform);
            _equipNotification.SetParent(_inventoryWindow.EquipButton.transform);
            _enhanceNotification.SetParent(_inventoryWindow.EnhanceAllButton.transform);
        }

    private void OnInventoryWindowClosed(BaseWindow baseWindow)
        {
            _itemNotification.RemoveParent();
            _equipNotification.RemoveParent();
            _enhanceNotification.RemoveParent();
        }

        private void OnItemAmountChanged(Item item)
        {
            CheckEnhance();
        }

        private void OnItemLevelUp(Item item)
        {
            CheckItem();
            CheckEquip();
            CheckEnhance();
        }
        
        private void OnEquip(StuffSlot slot)
        {
            CheckItem();
            CheckEquip();
        }

        private void OnItemUnlock(Item item) => 
            CheckItem();

        private void CheckEnhance()
        {
            bool canEnhance = CanAnyEnhance();
            
            if (canEnhance)
                _enhanceNotification.Enable();
            else
                _enhanceNotification.Disable();
        }

        private void CheckEquip()
        {
            if(!IsInventoryWithCurrentSlot) return;
            
            _effectiveItem = GetMostEffectiveItem();
            var selectedItem = _inventoryWindow.SelectedItem;
            if (_effectiveItem != null && _effectiveItem == selectedItem && !_effectiveItem.IsEquipped)
                _equipNotification.Enable();
            else
                _equipNotification.Disable();   
        }

        private void CheckItem()
        {
            _effectiveItem = GetMostEffectiveItem();
            if (_effectiveItem == null || _slot.Item == _effectiveItem)
                _itemNotification.Disable();
            else
            {
                _itemNotification.Enable();
                if (IsInventoryWithCurrentSlot) 
                    _itemNotification.SetParent(_inventoryWindow.GetViewByModel(_effectiveItem).transform);
            }
        }
        
        private bool CanAnyEnhance()
        {
            foreach (var item in _items)
            {
                if (!item.CanEnhance) continue;
                return true;
            }

            return false;
        }

        private StuffItem GetMostEffectiveItem()
        {
            BigDouble.Runtime.BigDouble maxValue = double.MinValue;
            StuffItem result = null;
            foreach (var item in _items)
            {
                if (!item.IsUnlocked) continue;
                if (item.IsEquipped)
                {
                    StuffSlot itemSlot = null;
                    foreach (var slot in _inventory.Slots)
                    {
                        if(slot.Item != item) continue;
                        itemSlot = slot;
                        break;
                    }
                    if(itemSlot != _slot) continue;
                }
                if (item.EquippedModifier.Value <= maxValue) continue;
                
                maxValue = item.EquippedModifier.Value;
                result = item;
            }

            return result;
        }
    }
}