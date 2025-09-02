using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.ContentControl;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Blessing;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Collection;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Companion;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Dailies;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.DailyRoulette;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Dungeon;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Phone;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Shop;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Skill;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Stuff;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Save;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Pool;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using VContainer;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model
{
    public class NotificationService : IDisposable
    {
        [Inject] private NotificationConfiguration _configuration;
        [Inject] private IObjectResolver _resolver;
        [Inject] private UIFactory _uiFactory;
        [Inject] private IAdService _adService;
        [Inject] private NotificationDataProvider _dataProvider;
        [Inject] private ContentService _contentService;

        private NotificationView _prefab;
        private NotificationDataContainer _dataContainer;

        private readonly List<INotificationProvider> _notificationProviders = new();

        private ObjectPoolWithParent<NotificationView> _notificationPool;

        [Preserve]
        public NotificationService()
        {
        }
        
        public async UniTask WarmUpAsync()
        {
            var notificationObject = await _uiFactory.LoadAsync(Asset.UI.NotificationView, false);
            _prefab = notificationObject.GetComponent<NotificationView>();
        }
        
        public void Initialize()
        {
            _dataContainer = _dataProvider.GetData();
            _notificationPool = new ObjectPoolWithParent<NotificationView>("NotificationPool", () =>
            {
                var instance = Object.Instantiate(_prefab);
                instance.Initialize(_adService);
                return instance;
            });

            CreateStuffNotification();
            CreateSkillNotification();
            CreateCompanionNotification();
            CreateDungeonNotification();
            CreateCollectionNotification();
            CreateShopNotification();
            CreateDailyRouletteNotification();
            CreateBlessingNotification();
            CreateDailiesNotification();
            CreatePhoneNotification();
        }

        //TODO USE T4 BY LLM HELP
        private void CreateStuffNotification()
        {
            var notificationProvider = new StuffNotificationProvider(_notificationPool);
            _resolver.Inject(notificationProvider);
            notificationProvider.Initialize();

            if (_configuration.StuffSetup.RootSetup.Enable)
            {
                var root = CreateRoot(_configuration.StuffSetup.RootSetup);
                root.AddProvider(notificationProvider);
            }
            
            _notificationProviders.Add(notificationProvider);
        }

        private void CreateSkillNotification()
        {
            var notificationProvider = new SkillNotificationHandler(_notificationPool);
            _resolver.Inject(notificationProvider);
            notificationProvider.Initialize();

            if (_configuration.SkillSetup.RootSetup.Enable)
            {
                var root = CreateRoot(_configuration.SkillSetup.RootSetup);
                root.AddProvider(notificationProvider);
            }
            
            _notificationProviders.Add(notificationProvider);
        }

        private void CreateCompanionNotification()
        {
            var notificationProvider = new CompanionNotificationHandler(_notificationPool);
            _resolver.Inject(notificationProvider);
            notificationProvider.Initialize();

            if (_configuration.CompanionSetup.RootSetup.Enable)
            {
                var root = CreateRoot(_configuration.CompanionSetup.RootSetup);
                root.AddProvider(notificationProvider);
            }
            
            _notificationProviders.Add(notificationProvider);
        }

        private void CreateDungeonNotification()
        {
            var notificationProvider = new SubModeNotificationHandler(_notificationPool);
            _resolver.Inject(notificationProvider);
            notificationProvider.Initialize();

            if (_configuration.SubModeSetup.RootSetup.Enable)
            {
                var root = CreateRoot(_configuration.SubModeSetup.RootSetup);
                root.AddProvider(notificationProvider);
            }
            
            _notificationProviders.Add(notificationProvider);
        }

        private void CreateCollectionNotification()
        {
            var collectionProvider = new CollectionNotificationHandler(_notificationPool);
            _resolver.Inject(collectionProvider);
            collectionProvider.Initialize();

            if (_configuration.CollectionSetup.RootSetup.Enable)
            {
                var root = CreateRoot(_configuration.CollectionSetup.RootSetup);
                root.AddProvider(collectionProvider);
            }
            
            _notificationProviders.Add(collectionProvider);
        }

        private void CreateShopNotification()
        {
            var content = _contentService.GetById(ContentConst.Shop);
            var notificationProvider = new ShopNotificationHandler(_notificationPool, _dataContainer, content);
            _resolver.Inject(notificationProvider);
            notificationProvider.Initialize();

            if (_configuration.ShopSetup.RootSetup.Enable)
            {
                var root = CreateRoot(_configuration.ShopSetup.RootSetup);
                root.AddProvider(notificationProvider);
            }
            
            _notificationProviders.Add(notificationProvider);
        }

        private void CreateDailyRouletteNotification()
        {
            var notificationProvider = new DailyRouletteNotificationHandler(_notificationPool);
            _resolver.Inject(notificationProvider);
            notificationProvider.Initialize();

            if (_configuration.DailyRouletteSetup.RootSetup.Enable)
            {
                var root = CreateRoot(_configuration.DailyRouletteSetup.RootSetup);
                root.AddProvider(notificationProvider);
            }
            
            _notificationProviders.Add(notificationProvider);
        }
        
        private void CreateBlessingNotification()
        {
            var notificationProvider = new BlessingNotificationHandler(_notificationPool);
            _resolver.Inject(notificationProvider);
            notificationProvider.Initialize();
            
            _notificationProviders.Add(notificationProvider);
        }

        private void CreateDailiesNotification()
        {
            var notificationProvider = new DailiesNotificationHandler(_notificationPool);
            _resolver.Inject(notificationProvider);
            notificationProvider.Initialize();

            if (_configuration.DailiesNotificationSetup.RootSetup.Enable)
            {
                var root = CreateRoot(_configuration.DailiesNotificationSetup.RootSetup);
                root.AddProvider(notificationProvider);
            }
            
            _notificationProviders.Add(notificationProvider);
        }

        private void CreatePhoneNotification()
        {
            var notificationProvider = new PhoneNotificationProvider(_notificationPool);
            _resolver.Inject(notificationProvider);
            notificationProvider.Initialize();
            if (_configuration.PhoneNotificationSetup.RootSetup.Enable)
            {
                var root = CreateRoot(_configuration.PhoneNotificationSetup.RootSetup);
                root.AddProvider(notificationProvider);
            }
            
            _notificationProviders.Add(notificationProvider);
        }

        private ButtonNotification CreateRoot(RootButtonSetup buttonSetup)
        {
            var root = new ButtonNotification(buttonSetup.ButtonId, buttonSetup.NotificationSetup, _notificationPool);
            _resolver.Inject(root);
            root.Initialize();
            return root;
        }

        public void Dispose()
        {
            foreach (var notificationProvider in _notificationProviders) 
                notificationProvider.Dispose();
        }
    }
}