using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Pool;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Phone
{
    public class PhoneNotificationProvider : INotificationProvider
    {
        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private ChatNotificationHandler _chatNotificationHandler;

        [Inject] private IObjectResolver _resolver;
        
        public event Action<INotificationProvider> OnMainNotificationChanged;

        public PhoneNotificationProvider(ObjectPoolWithParent<NotificationView> pool) => 
            _pool = pool;

        public void Initialize()
        {
            _chatNotificationHandler = new ChatNotificationHandler(_pool);
            _resolver.Inject(_chatNotificationHandler);
            _chatNotificationHandler.Initialize();
        }
        
        public void FillMainNotifications(in List<Notification> notifications) => 
            notifications.Add(_chatNotificationHandler.ChatIconNotification);

        public void Dispose() => 
            _chatNotificationHandler.Dispose();
    }
}