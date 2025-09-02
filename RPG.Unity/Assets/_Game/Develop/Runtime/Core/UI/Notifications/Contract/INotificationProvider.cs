using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract
{
    public interface INotificationProvider : IDisposable
    {
        event Action<INotificationProvider> OnMainNotificationChanged;
        void FillMainNotifications(in List<Notification> notifications);
    }
}