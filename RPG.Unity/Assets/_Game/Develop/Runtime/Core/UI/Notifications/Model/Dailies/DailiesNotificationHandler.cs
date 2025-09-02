using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model;
using PleasantlyGames.RPG.Runtime.Core.Dailies.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Model;
using PleasantlyGames.RPG.Runtime.Pool;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.Dailies
{
    public class DailiesNotificationHandler : INotificationProvider
    {
        [Inject] private NotificationConfiguration _configuration;
        [Inject] private DailyService _dailyService;
        [Inject] private IWindowService _windowService;
        [Inject] private VipService _vipService;

        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly string _windowType = nameof(DailiesWindow);
        private readonly Dictionary<Daily, Notification> _notifications = new();
        private readonly ObjectPoolWithParent<NotificationView> _pool;

        public event Action<INotificationProvider> OnMainNotificationChanged;
        
        public DailiesNotificationHandler(ObjectPoolWithParent<NotificationView> pool) => 
            _pool = pool;

        public void Initialize()
        {
            foreach (var daily in _dailyService.Dailies)
            {
                var notification = new Notification(_pool, _configuration.DailiesNotificationSetup.ClaimSetup, _configuration.DailiesNotificationSetup.ImageSetup);
                _notifications.Add(daily, notification);
            }
            
            if (_windowService.IsExist<DailiesWindow>()) 
                HandleWindow().Forget();
            else
                _windowService.OnCreate += OnWindowCreate;

            _dailyService.OnDailyCompleted += OnDailyCompleted;
            _dailyService.OnDailyCollected += OnDailyCollected;
            _dailyService.OnResetProgress += OnResetProgress;

            _vipService.IsActive
                .Skip(1)
                .Subscribe(_ => CheckDailies())
                .AddTo(_compositeDisposable);
            
            CheckDailies();
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
            _dailyService.OnDailyCompleted -= OnDailyCompleted;
            _dailyService.OnDailyCollected -= OnDailyCollected;
            _dailyService.OnResetProgress -= OnResetProgress;
        }

        private void OnDailyCollected(Daily daily) => CheckDailies();
        private void OnDailyCompleted(Daily daily) => CheckDailies();
        private void OnResetProgress() => CheckDailies();

        private async void OnWindowCreate(string windowType)
        {
            if(windowType != _windowType) return;
            _windowService.OnCreate -= OnWindowCreate;
            await HandleWindow();
        }

        private async UniTask HandleWindow()
        {
            var window = await _windowService.GetAsync<DailiesWindow>(false);
            foreach (var kvp in _notifications)
            {
                var view = window.GetViewByModel(kvp.Key);
                if (view == null) continue;
                kvp.Value.SetParent(view.Button.transform);
            }
        }

        private void CheckDailies()
        {
            foreach (var (daily, value) in _notifications)
            {
                if (!_vipService.IsActive.CurrentValue && daily.IsBonus)
                {
                    value.Disable();
                    continue;
                }
                
                if(daily.HasReward)
                    value.Enable();
                else
                    value.Disable();
            }
        }

        public void FillMainNotifications(in List<Notification> notifications)
        {
            foreach (var kvp in _notifications) 
                notifications.Add(kvp.Value);
        }
    }
}