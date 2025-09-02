using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config
{
    [Serializable]
    public class NotificationImageSetup
    {
        public NotificationImageType ImageType = NotificationImageType.Default;
        public Vector2 Size = new Vector2(40f, 40f);
    }
}