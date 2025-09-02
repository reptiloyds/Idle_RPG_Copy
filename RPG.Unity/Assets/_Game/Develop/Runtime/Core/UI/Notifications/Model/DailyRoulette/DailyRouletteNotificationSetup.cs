using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.DailyRoulette
{
    [Serializable]
    public class DailyRouletteNotificationSetup
    {
        public RootButtonSetup RootSetup;
        public NotificationImageSetup ImageSetup;
        public NotificationSetup SpinSettup;
    }
}