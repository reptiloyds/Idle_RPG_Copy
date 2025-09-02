using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Pool;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Stuff
{
    public class StuffNotificationProvider : INotificationProvider
    {
        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private readonly Dictionary<Branch, List<StuffNotificationHandler>> _dictionary = new();
        private Branch _currentBranch;

        [Inject] private BranchService _branchService;
        [Inject] private StuffInventory _stuffInventory;
        [Inject] private IObjectResolver _resolver;

        public event Action<INotificationProvider> OnMainNotificationChanged;

        public StuffNotificationProvider(ObjectPoolWithParent<NotificationView> pool) =>
            _pool = pool;

        public void Initialize()
        {
            foreach (var branch in _branchService.Branches)
            {
                _dictionary[branch] = new List<StuffNotificationHandler>();
                var slots = _stuffInventory.Slots.Where(x => string.Equals(x.BranchId, branch.Id));
                foreach (var slot in slots)
                {
                    var notification = new StuffNotificationHandler(_pool, slot);
                    _resolver.Inject(notification);
                    notification.Initialize();
                    _dictionary[branch].Add(notification);
                }
            }

            _branchService.SwitchBranch += OnBranchSwitched;
            SwitchNotifications();
        }

        public void Dispose()
        {
            _branchService.SwitchBranch -= OnBranchSwitched;
        }

        private async void OnBranchSwitched()
        {
            await UniTask.NextFrame(); // TODO : rewrok it
            SwitchNotifications();
        }

        public void FillMainNotifications(in List<Notification> notifications)
        {
            var notificationHandlers = _dictionary[_currentBranch];
            foreach (var notificationHandler in notificationHandlers)
                notifications.Add(notificationHandler.SlotNotification);
        }

        private void SwitchNotifications()
        {
            if (_currentBranch != null)
            {
                var prevNotifications = _dictionary[_currentBranch];
                foreach (var notification in prevNotifications)
                    notification.Disable();
            }

            _currentBranch = _branchService.GetSelectedBranch();
            var notifications = _dictionary[_currentBranch];
            foreach (var notification in notifications)
                notification.Enable();

            OnMainNotificationChanged?.Invoke(this); 
        }
    }
}