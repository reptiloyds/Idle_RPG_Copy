using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Shop
{
    [Serializable]
    public class ShopNotificationSetup
    {
        public RootButtonSetup RootSetup;

        public NotificationSetup LootboxSwitcherSetup;
        public NotificationSetup LootboxSetup;
        public NotificationImageSetup LootboxImageSetup;
        
        public NotificationSetup ProductSwitcherSetup;
        public NotificationSetup ProductSetup;
        public NotificationImageSetup ProductImageSetup;
        
        public NotificationSetup PeriodicProductSwitcherSetup;
        public NotificationSetup PeriodicProductTabSetup;
        public NotificationSetup PeriodicProductSetup;
        public NotificationImageSetup PeriodicProductImageSetup;
    }
}