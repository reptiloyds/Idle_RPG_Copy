using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Model.Periodic;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.Core.Products.View;
using PleasantlyGames.RPG.Runtime.Core.Products.View.Periodic;
using PleasantlyGames.RPG.Runtime.Core.ShopHub.Type;
using PleasantlyGames.RPG.Runtime.Core.ShopHub.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Save;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Pool;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Shop
{
    public class ShopNotificationHandler : INotificationProvider
    {
        [Inject] private LootboxService _lootboxService;
        [Inject] private IWindowService _windowService;
        [Inject] private NotificationConfiguration _configuration;
        [Inject] private ProductService _productService;
        [Inject] private VipService _vipService;

        private const string WindowType = nameof(ShopHubWindow);
        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private readonly NotificationData _data;
        private readonly IUnlockable _unlockable;
        private readonly Dictionary<Lootbox, Notification> _lootboxNotifications = new();
        private readonly Dictionary<PeriodicType, Notification> _periodicTabNotifications = new();
        private readonly Dictionary<Product, Notification> _productNotifications = new();
        private Notification _lootboxSwitch;
        private Notification _permanentProductLook;
        private Notification _permanentProductSwitch;
        private Notification _periodicProductSwitch;
        private readonly CompositeDisposable _compositeDisposable = new();
        private ShopHubWindow _shopWindow;

        private bool IsUnlocked => _unlockable == null || _unlockable.IsUnlocked;
        private const string DataKey = "ShopNotifications";
        private const string ProductCompleteKey = "ProductSwitch";

        public event Action<INotificationProvider> OnMainNotificationChanged;

        public ShopNotificationHandler(ObjectPoolWithParent<NotificationView> pool, NotificationDataContainer container,
            IUnlockable unlockable)
        {
            _pool = pool;
            if (!container.Dictionary.TryGetValue(DataKey, out var data))
            {
                data = new NotificationData();
                container.Dictionary.Add(DataKey, data);
            }

            _data = data;
            _unlockable = unlockable;
        }

        public void Initialize()
        {
            var shopSetup = _configuration.ShopSetup;
            _lootboxSwitch = new Notification(_pool, shopSetup.LootboxSwitcherSetup);
            foreach (var lootbox in _lootboxService.Lootboxes)
            {
                var notification = new Notification(_pool, shopSetup.LootboxSetup, shopSetup.LootboxImageSetup);
                _lootboxNotifications.Add(lootbox, notification);
                _lootboxSwitch.AppendChild(notification);

                lootbox.OnUnlocked += OnUnlock;
                lootbox.BonusOpenHandler.IsCooldown
                    .Subscribe(_ => CheckLootbox(lootbox))
                    .AddTo(_compositeDisposable);
            }
            
            _vipService.IsActive
                .Skip(1)
                .Subscribe(_ => CheckLootboxes())
                .AddTo(_compositeDisposable);

            _permanentProductSwitch = new Notification(_pool, shopSetup.ProductSwitcherSetup, shopSetup.ProductImageSetup);
            _permanentProductLook = new Notification(_pool, shopSetup.ProductSwitcherSetup, shopSetup.ProductImageSetup);
            _permanentProductSwitch.AppendChild(_permanentProductLook);
            _periodicProductSwitch = new Notification(_pool, shopSetup.PeriodicProductSwitcherSetup,
                shopSetup.PeriodicProductImageSetup);

            foreach (var product in _productService.Products)
            {
                if (product.Placement != ProductPlacement.Permanent) continue;
                var notification = new Notification(_pool, shopSetup.ProductSetup, shopSetup.ProductImageSetup);
                _productNotifications.Add(product, notification);
                _permanentProductSwitch.AppendChild(notification);
                SubscribeOnProduct(product);
            }

            foreach (var periodKvp in _productService.PeriodicProducts.Dictionary)
            {
                var tabNotification = new Notification(_pool, shopSetup.PeriodicProductTabSetup,
                    shopSetup.PeriodicProductImageSetup);
                _periodicTabNotifications.Add(periodKvp.Key, tabNotification);
                _periodicProductSwitch.AppendChild(tabNotification);
                foreach (var orderKvp in periodKvp.Value)
                foreach (var product in orderKvp.Value)
                {
                    var productNotification = new Notification(_pool, shopSetup.PeriodicProductSetup,
                        shopSetup.PeriodicProductImageSetup);
                    _productNotifications.Add(product, productNotification);
                    tabNotification.AppendChild(productNotification);
                    SubscribeOnProduct(product);
                }
            }

            if (_windowService.IsExist<ShopHubWindow>())
                HandleWindow().Forget();
            else
                _windowService.OnCreate += OnWindowCreate;

            if (IsUnlocked)
            {
                CheckLootboxes();
                CheckProductSwitcher();
                CheckProducts();
            }
            else
                _unlockable.OnUnlocked += OnUnlocked;

            void SubscribeOnProduct(Product product)
            {
                if (!product.Access.IsUnlocked)
                    product.Access.OnUnlocked += OnProductUnlocked;

                product.OnPurchaseHandled += CheckProduct;
                product.Price.IsFree
                    .Subscribe(_ => CheckProduct(product))
                    .AddTo(_compositeDisposable);

                product.Price.Cooldown
                    .Pairwise()
                    .Where((tuple) => tuple is { Previous: <= 0, Current: > 0 } or { Previous: > 0, Current: <= 0 })
                    .Subscribe(_ => CheckProduct(product))
                    .AddTo(_compositeDisposable);
            }
        }

        public void Dispose()
        {
            foreach (var kvp in _lootboxNotifications)
                kvp.Key.OnUnlocked -= OnUnlock;
            _compositeDisposable.Dispose();

            if (!_windowService.IsExist<ShopHubWindow>())
                _windowService.OnCreate -= OnWindowCreate;
        }

        private void OnProductUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlocked;
            var product = ((ProductAccess)unlockable).Product;
            CheckProduct(product);
        }

        private void OnUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlocked;
            CheckLootboxes();
            CheckProductSwitcher();
            CheckProducts();
        }

        private async void OnWindowCreate(string windowType)
        {
            if (windowType != WindowType) return;
            _windowService.OnCreate -= OnWindowCreate;

            await HandleWindow();
        }

        private async UniTask HandleWindow()
        {
            _shopWindow = await _windowService.GetAsync<ShopHubWindow>(false);
            var lootboxCollectionView = _shopWindow.LootboxCollectionView;
            foreach (var kvp in _lootboxNotifications)
            {
                var view = lootboxCollectionView.GetViewByModel(kvp.Key);
                kvp.Value.SetParent(view.BonusButton.transform);
            }

            foreach (var view in _shopWindow.PermanentProductCollectionView.Views)
            {
                if (_productNotifications.TryGetValue(view.Product, out var notification))
                    notification.SetParent(view.Button.transform);
            }

            foreach (var tabButton in _shopWindow.PeriodicProductCollectionsView.Buttons)
                if (_periodicTabNotifications.TryGetValue(tabButton.Type, out var notification))
                    notification.SetParent(tabButton.transform);

            _shopWindow.PeriodicProductCollectionsView.OnNewProducts += OnNewPeriodicProducts;
            foreach (var kvp in _shopWindow.PeriodicProductCollectionsView.ViewDictionary)
            foreach (var view in kvp.Value)
            {
                if (_productNotifications.TryGetValue(view.Product, out var notification))
                    notification.SetParent(view.Button.transform);
            }

            var productButton = _shopWindow.GetButton(ShopElement.PermanentProduct);
            _lootboxSwitch.SetParent(_shopWindow.GetButton(ShopElement.Lootbox).transform);
            _permanentProductSwitch.SetParent(productButton.transform);
            _periodicProductSwitch.SetParent(_shopWindow.GetButton(ShopElement.PeriodicProduct).transform);

            if (!_data.Completed.ContainsKey(ProductCompleteKey))
                productButton.OnClick += OnProductSwitchClick;
        }

        private void OnNewPeriodicProducts(PeriodicType type)
        {
            if (!_shopWindow.PeriodicProductCollectionsView.ViewDictionary.TryGetValue(type, out var views))
                return;

            foreach (var productView in views)
            {
                var product = productView.Product;
                if (_productNotifications.TryGetValue(product, out var notification))
                    notification.SetParent(productView.Button.transform);
            }
        }

        private async void OnProductSwitchClick()
        {
            var window = await _windowService.GetAsync<ShopHubWindow>(false);
            window.GetButton(ShopElement.PermanentProduct).OnClick -= OnProductSwitchClick;
            _data.Completed.Add(ProductCompleteKey, true);
            _permanentProductLook.Disable();
        }

        public void FillMainNotifications(in List<Notification> notifications)
        {
            notifications.Add(_lootboxSwitch);
            notifications.Add(_permanentProductSwitch);
            notifications.Add(_periodicProductSwitch);
        }

        private void OnUnlock(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlock;
            CheckLootbox((Lootbox)unlockable);
        }

        private void CheckLootboxes()
        {
            foreach (var kvp in _lootboxNotifications)
                CheckLootbox(kvp.Key);
        }

        private void CheckProducts()
        {
            foreach (var kvp in _productNotifications)
                CheckProduct(kvp.Key);
        }

        private void CheckProduct(Product product)
        {
            if (!IsUnlocked) return;
            var notification = _productNotifications[product];
            if (!product.Access.IsUnlocked)
                notification.Disable();
            else
            {
                if (product.Price.IsFree.CurrentValue && !product.Access.LimitIsOver &&
                    product.Price.Cooldown.CurrentValue <= 0)
                    notification.Enable();
                else
                    notification.Disable();
            }
        }

        private void CheckProductSwitcher()
        {
            if (!IsUnlocked) return;
            if (_data.Completed.ContainsKey(ProductCompleteKey))
                _permanentProductLook.Disable();
            else
                _permanentProductLook.Enable();
        }

        private void CheckLootbox(Lootbox lootbox)
        {
            if (!IsUnlocked) return;
            var notification = _lootboxNotifications[lootbox];
            if (lootbox.IsUnlocked && lootbox.BonusOpenHandler.OpenAmount.CurrentValue > 0 && !lootbox.BonusOpenHandler.IsCooldown.CurrentValue && _vipService.IsActive.CurrentValue)
                notification.Enable();
            else
                notification.Disable();
        }
    }
}