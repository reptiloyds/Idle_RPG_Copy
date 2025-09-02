using System;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config
{
    [Serializable]
    public class NotificationSetup
    {
        public Vector2 AnchorMin = new Vector2(0, 1);
        public Vector2 AnchorMax = new Vector2(0, 1);
        public Vector2 AnchoredPosition = new Vector2(10, -10);
    }
}