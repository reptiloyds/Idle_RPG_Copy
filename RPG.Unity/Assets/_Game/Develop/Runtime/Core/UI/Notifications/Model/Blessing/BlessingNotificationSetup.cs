using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Blessing
{
    [Serializable]
    public class BlessingNotificationSetup
    {
        public NotificationImageSetup ImageSetup;
        public NotificationSetup ButtonSetup;
    }
}