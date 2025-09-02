using System;
using Sirenix.OdinInspector;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config
{
    [Serializable]
    public class RootButtonSetup
    {
        public bool Enable;
        [HideIf("@this.Enable == false")]
        public string ButtonId;
        [HideIf("@this.Enable == false")]
        public NotificationSetup NotificationSetup;
    }
}