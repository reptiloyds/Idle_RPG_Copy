using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Blessing;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Collection;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Companion;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Dailies;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.DailyRoulette;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Dungeon;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Phone;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Shop;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Skill;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Stuff;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config
{
    [Serializable]
    public class NotificationConfiguration
    {
        public StuffNotificationSetup StuffSetup;
        public SkillNotificationSetup SkillSetup;
        public CompanionNotificationSetup CompanionSetup;
        public SubModeNotificationSetup SubModeSetup;
        public CollectionNotificationSetup CollectionSetup;
        public ShopNotificationSetup ShopSetup;
        public DailyRouletteNotificationSetup DailyRouletteSetup;
        public BlessingNotificationSetup BlessingNotificationSetup;
        public DailiesNotificationSetup DailiesNotificationSetup;
        public PhoneNotificationSetup PhoneNotificationSetup;
    }
}