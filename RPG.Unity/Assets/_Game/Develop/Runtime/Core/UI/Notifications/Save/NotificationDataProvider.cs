using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Save
{
    [Serializable]
    public class NotificationDataContainer
    {
        public Dictionary<string, NotificationData> Dictionary = new();
        
        [Preserve]
        public NotificationDataContainer()
        {
        }
    }

    [Serializable]
    public class NotificationData
    {
        public Dictionary<string, bool> Completed = new();
        [Preserve]
        public NotificationData()
        {
        }
    }
    
    public class NotificationDataProvider : BaseDataProvider<NotificationDataContainer>
    {
        [Preserve]
        public NotificationDataProvider() : base(loadOnInitialize: true)
        {
        }

        public override void LoadData()
        {
            base.LoadData();
            Data ??= new NotificationDataContainer();
        }
    }
}