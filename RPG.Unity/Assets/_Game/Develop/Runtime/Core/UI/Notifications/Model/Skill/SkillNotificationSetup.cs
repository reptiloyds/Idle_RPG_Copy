using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Skill
{
    [Serializable]
    public class SkillNotificationSetup
    {
        public RootButtonSetup RootSetup;
        public NotificationImageSetup ImageSetup;
        public NotificationSetup EnhanceSetup;
    }
}