using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Pool;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Blessing
{
    public class BlessingNotificationHandler : INotificationProvider
    {
        [Inject] private NotificationConfiguration _configuration;
        [Inject] private BlessingService _blessingService;
        [Inject] private BlessingButton _blessingButton;
        
        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private Notification _notification;
        
        public event Action<INotificationProvider> OnMainNotificationChanged;

        public BlessingNotificationHandler(ObjectPoolWithParent<NotificationView> pool) => 
            _pool = pool;

        public void Initialize()
        {
            _notification = new Notification(_pool, _configuration.BlessingNotificationSetup.ButtonSetup, _configuration.BlessingNotificationSetup.ImageSetup);
            _notification.SetParent(_blessingButton.Visual);
            
            _blessingService.OnBlessingEnabled += OnBlessingStatChanged;
            _blessingService.OnBlessingDisabled += OnBlessingStatChanged;
            CheckBlessings();
        }
        
        public void Dispose()
        {
            _blessingService.OnBlessingEnabled -= OnBlessingStatChanged;
            _blessingService.OnBlessingDisabled -= OnBlessingStatChanged;
        }

        private void OnBlessingStatChanged(BlessingFeature.Model.Blessing blessing) => 
            CheckBlessings();

        private void CheckBlessings()
        {
            foreach (var blessing in _blessingService.Blessings)
            {
                if (blessing.IsActive.CurrentValue) continue;
                _notification.Enable();
                return;
            }
            
            _notification.Disable();
        }

        public void FillMainNotifications(in List<Notification> notifications) => 
            notifications.Add(_notification);
    }
}