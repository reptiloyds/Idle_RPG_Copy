using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Collection
{
    [Serializable]
    public class CollectionNotificationSetup
    {
        public RootButtonSetup RootSetup;
        
        public NotificationImageSetup ImageSetup;
        public NotificationSetup EnhanceSetup;
        public NotificationSetup BookmarkSetup;
    }
}