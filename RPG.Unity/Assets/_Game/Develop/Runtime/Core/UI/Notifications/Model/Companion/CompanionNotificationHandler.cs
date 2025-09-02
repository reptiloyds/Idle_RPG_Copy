using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Pool;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Companion
{
    public class CompanionNotificationHandler : INotificationProvider
    {
        [Inject] private CompanionInventory _inventory;
        [Inject] private IWindowService _windowService;
        [Inject] private NotificationConfiguration _configuration;
        
        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private readonly string _windowType = nameof(CompanionInventoryWindow);
        
        private Notification _enhanceNotification;
        private IReadOnlyList<CompanionItem> _items;
        
        public event Action<INotificationProvider> OnMainNotificationChanged;

        public CompanionNotificationHandler(ObjectPoolWithParent<NotificationView> pool) => 
            _pool = pool;
        
        public void Initialize()
        {
            _enhanceNotification = new Notification(_pool, _configuration.SkillSetup.EnhanceSetup, _configuration.SkillSetup.ImageSetup);
            if (_windowService.IsExist<CompanionInventoryWindow>())
                HandleWindow().Forget();
            else
                _windowService.OnCreate += OnWindowCreate;
            _items = _inventory.Items;
            foreach (var item in _items)
            {
                item.OnAmountChanged += OnItemAmountChanged;
                item.OnLevelUp += OnItemLevelUp;
            }

            CheckEnhance();
        }

        public void Dispose()
        {
            foreach (var item in _items)
            {
                item.OnAmountChanged -= OnItemAmountChanged;
                item.OnLevelUp -= OnItemLevelUp;
            }
            _windowService.OnCreate -= OnWindowCreate;
        }
        
        public void FillMainNotifications(in List<Notification> notifications) => 
            notifications.Add(_enhanceNotification);

        private void OnItemAmountChanged(Item item) => 
            CheckEnhance();

        private void OnItemLevelUp(Item item) => 
            CheckEnhance();

        private async void OnWindowCreate(string windowType)
        {
            if(windowType != _windowType) return;
            _windowService.OnCreate -= OnWindowCreate;
            await HandleWindow();
        }

        private async UniTask HandleWindow()
        {
            var window = await _windowService.GetAsync<CompanionInventoryWindow>(false);
            _enhanceNotification.SetParent(window.EnhanceAllButton.transform);
        }

        private void CheckEnhance()
        {
            bool canEnhance = CanAnyEnhance();
            
            if (canEnhance)
                _enhanceNotification.Enable();
            else
                _enhanceNotification.Disable();
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
    }
}