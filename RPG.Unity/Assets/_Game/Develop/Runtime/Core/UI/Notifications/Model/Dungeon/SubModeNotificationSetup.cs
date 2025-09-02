using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Dungeon
{
    [Serializable]
    public class SubModeNotificationSetup
    {
        public RootButtonSetup RootSetup;
        public NotificationImageSetup ImageSetup;
        public NotificationSetup EnterSetup;
    }
}