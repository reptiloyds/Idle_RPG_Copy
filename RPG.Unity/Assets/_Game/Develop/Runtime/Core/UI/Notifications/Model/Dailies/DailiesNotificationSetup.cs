using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Dailies
{
    [Serializable]
    public class DailiesNotificationSetup
    {
        public RootButtonSetup RootSetup;
        public NotificationImageSetup ImageSetup;
        public NotificationSetup ClaimSetup;
    }
}