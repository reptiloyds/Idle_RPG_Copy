using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Pool;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model.DailyRoulette
{
    public class DailyRouletteNotificationHandler : INotificationProvider
    {
        [Inject] private Roulette.Daily.Model.DailyRoulette _dailyRoulette;
        [Inject] private NotificationConfiguration _configuration;
        [Inject] private IWindowService _windowService;
        [Inject] private VipService _vipService;

        private readonly string _windowType = nameof(DailyRouletteWindow);
        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private Notification _notification;
        private CompositeDisposable _compositeDisposable = new();

        public event Action<INotificationProvider> OnMainNotificationChanged;
        
        public DailyRouletteNotificationHandler(ObjectPoolWithParent<NotificationView> pool) => 
            _pool = pool;

        public void Initialize()
        {
            _notification = new Notification(_pool, _configuration.DailyRouletteSetup.SpinSettup, _configuration.DailyRouletteSetup.ImageSetup);

            if (_windowService.IsExist<DailyRouletteWindow>()) 
                HandleWindow().Forget();
            else
                _windowService.OnCreate += OnWindowCreate;

            _dailyRoulette.OnSpin += CheckSpin;
            _dailyRoulette.IsCooldown
                .Subscribe(_ => CheckSpin())
                .AddTo(_compositeDisposable);
            _vipService.IsActive
                .Skip(1)
                .Subscribe(_ => CheckSpin())
                .AddTo(_compositeDisposable);
            CheckSpin();
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
            _dailyRoulette.OnSpin -= CheckSpin;
            if (!_windowService.IsExist<DailyRouletteWindow>()) 
                _windowService.OnCreate -= OnWindowCreate;
        }

        private async void OnWindowCreate(string windowType)
        {
            if(windowType != _windowType) return;
            _windowService.OnCreate -= OnWindowCreate;
            await HandleWindow();
        }

        private async UniTask HandleWindow()
        {
            var window = await _windowService.GetAsync<DailyRouletteWindow>(false);
            _notification.SetParent(window.SpinButton.transform);
        }

        private void CheckSpin()
        {
            if (_dailyRoulette.SpinAmount.CurrentValue > 0 && !_dailyRoulette.IsCooldown.CurrentValue)
            {
                if (_dailyRoulette.FreeSpinAmount.CurrentValue <= 0)
                    if (_vipService.IsActive.CurrentValue)
                        _notification.Enable();
                    else
                        _notification.Disable();
                else
                    _notification.Enable();
            }
            else
                _notification.Disable();
        }
        
        public void FillMainNotifications(in List<Notification> notifications) => 
            notifications.Add(_notification);
    }
}