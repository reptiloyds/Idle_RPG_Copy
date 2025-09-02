using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Stuff
{
    [Serializable]
    public class StuffNotificationSetup
    {
        public RootButtonSetup RootSetup;
        public NotificationImageSetup CommonImageSetup;
        
        public NotificationSetup ItemSetup;
        public NotificationSetup SlotSetup;
        public NotificationSetup EquipSetup;
        public NotificationSetup EnhanceSetup;
    }
}