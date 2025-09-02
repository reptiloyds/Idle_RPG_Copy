using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Companion
{
    [Serializable]
    public class CompanionNotificationSetup
    {
        public RootButtonSetup RootSetup;
        public NotificationImageSetup ImageSetup;
        public NotificationSetup EnhanceSetup;
    }
}