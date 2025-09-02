using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Collections.Model;
using PleasantlyGames.RPG.Runtime.Core.Collections.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Pool;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Collection
{
    public class CollectionNotificationHandler : INotificationProvider
    {
        [Inject] private CollectionService _collectionService;
        [Inject] private NotificationConfiguration _configuration;
        [Inject] private IWindowService _windowService;

        private readonly string _windowType = nameof(CollectionWindow);
        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private readonly Dictionary<Collections.Model.Collection, Notification> _notifications = new ();
        private readonly Dictionary<ItemType, Notification> _bookmarkNotifications = new ();

        public event Action<INotificationProvider> OnMainNotificationChanged;
        
        public CollectionNotificationHandler(ObjectPoolWithParent<NotificationView> pool) => 
            _pool = pool;

        public void Initialize()
        {
            foreach (var collection in _collectionService.Collections)
            {
                var notification = new Notification(_pool, _configuration.CollectionSetup.EnhanceSetup, _configuration.CollectionSetup.ImageSetup);
                _notifications.Add(collection, notification);
                if (_bookmarkNotifications.TryGetValue(collection.Type, out var bookmarkNot))
                    bookmarkNot.AppendChild(notification);
                else
                {
                    var bookmarkNotification = new Notification(_pool, _configuration.CollectionSetup.BookmarkSetup);
                    bookmarkNotification.AppendChild(notification);
                    _bookmarkNotifications.Add(collection.Type, bookmarkNotification);
                }

                collection.OnUpdate += CheckEnhance;
            }

            if (_windowService.IsExist<CollectionWindow>()) 
                HandleWindow().Forget();
            else
                _windowService.OnCreate += OnWindowCreate;

            foreach (var kvp in _notifications) 
                CheckEnhance(kvp.Key);
        }

        public void Dispose()
        {
            foreach (var kvp in _notifications) 
                kvp.Key.OnUpdate -= CheckEnhance;
            
            if (!_windowService.IsExist<CollectionWindow>()) 
                _windowService.OnCreate -= OnWindowCreate;
        }

        private async void OnWindowCreate(string windowType)
        {
            if(windowType != _windowType) return;
            _windowService.OnCreate -= OnWindowCreate;
            await HandleWindow();
        }

        private async UniTask HandleWindow()
        {
            var window = await _windowService.GetAsync<CollectionWindow>(false);
            foreach (var kvp in _notifications)
            {
                var view = window.GetViewByModel(kvp.Key);
                kvp.Value.SetParent(view.EnhanceButton.transform);
            }

            foreach (var kvp in _bookmarkNotifications)
            {
                var button = window.GetBookmarkButton(kvp.Key);
                kvp.Value.SetParent(button.transform);
            }
        }

        private void CheckEnhance(Collections.Model.Collection collection)
        {
            if (collection.CanEnhance)
                _notifications[collection].Enable();
            else
                _notifications[collection].Disable();
        }
        
        public void FillMainNotifications(in List<Notification> notifications)
        {
            foreach (var kvp in _bookmarkNotifications) 
                notifications.Add(kvp.Value);
        }
    }
}