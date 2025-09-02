using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.View;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Pool;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Dungeon
{
    public class SubModeNotificationHandler : INotificationProvider
    {
        [Inject] private DungeonModeFacade _dungeonModeFacade;
        [Inject] private RouletteFacade _rouletteFacade;
        
        [Inject] private NotificationConfiguration _configuration;
        [Inject] private SubModesPage _subModesPage;

        private readonly Dictionary<SubMode, Notification> _notifications = new();
        private readonly ObjectPoolWithParent<NotificationView> _pool;
        
        public event Action<INotificationProvider> OnMainNotificationChanged;

        public SubModeNotificationHandler(ObjectPoolWithParent<NotificationView> pool) => 
            _pool = pool;

        void INotificationProvider.FillMainNotifications(in List<Notification> notifications)
        {
            foreach (var kvp in _notifications) 
                notifications.Add(kvp.Value);
        }

        public void Initialize()
        {
            foreach (var dungeon in _dungeonModeFacade.Dungeons)
            {
                dungeon.EnterResource.OnChange += OnEnterResourceChanged;
                var notification = new Notification(_pool, _configuration.SubModeSetup.EnterSetup, _configuration.SubModeSetup.ImageSetup);
                _notifications.Add(dungeon, notification);
                var view = _subModesPage.GetViewByModel(dungeon);
                notification.SetParent(view.EnterButton.transform);
            }

            foreach (var roulette in _rouletteFacade.RouletteModes)
            {
                roulette.EnterResource.OnChange += OnEnterResourceChanged;
                var notification = new Notification(_pool, _configuration.SubModeSetup.EnterSetup, _configuration.SubModeSetup.ImageSetup);
                _notifications.Add(roulette, notification);
                var view = _subModesPage.GetViewByModel(roulette);
                notification.SetParent(view.EnterButton.transform);
            }

            CheckEnter();
        }

        public void Dispose()
        {
            foreach (var kvp in _notifications) 
                kvp.Key.EnterResource.OnChange -= OnEnterResourceChanged;   
        }

        private void OnEnterResourceChanged() => 
            CheckEnter();

        private void CheckEnter()
        {
            foreach (var kvp in _notifications)
            {
                if (kvp.Key.IsEnterResourceEnough)
                    kvp.Value.Enable();
                else 
                    kvp.Value.Disable();
            }
        }
    }
}