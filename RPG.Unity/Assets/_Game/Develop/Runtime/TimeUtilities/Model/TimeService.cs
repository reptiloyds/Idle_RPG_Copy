using System;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Save;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.TimeUtilities.Model
{
    public class TimeService : ITickable, IDisposable
    {
        [Inject] private TimeDataProvider _dataProvider;

        private TimeDataContainer _data;
        private DateTime _serverStatTime;
        private float _localStartupTime;
        private readonly SerialDisposable _dayTimer = new();
        private readonly SerialDisposable _weekTimer = new();
        private readonly SerialDisposable _monthTimer = new();
        private readonly ReactiveProperty<float> _timeToEndDay = new();
        private readonly ReactiveProperty<float> _timeToEndWeek = new();
        private readonly ReactiveProperty<float> _timeToEndMonth = new();
        
        public ReadOnlyReactiveProperty<float> TimeToEndDay => _timeToEndDay;
        public ReadOnlyReactiveProperty<float> TimeToEndWeek => _timeToEndWeek;
        public ReadOnlyReactiveProperty<float> TimeToEndMonth => _timeToEndMonth;
        public bool IsInitialized { get; private set; }
        public bool IsFirstSessionToday { get; private set; }
        public float TotalPlaytime => _data.TotalPlaytime;

        public event Action OnNewDay;
        public event Action OnNewWeek;
        public event Action OnNewMonth;

        [UnityEngine.Scripting.Preserve]
        protected TimeService()
        {
        }

        public void SetupServerStartTime(DateTime time) =>
            _serverStatTime = time;

        public void Initialize()
        {
            _data = _dataProvider.GetData();
            _dataProvider.OnUpdateData += OnUpdateDataProvider;

            _localStartupTime = Time.realtimeSinceStartup;

            if (_data.SaveTime == default)
            {
                _data.SaveTime = Now();
                IsFirstSessionToday = true;
            }
            else
                IsFirstSessionToday = _data.SaveTime.Date < Now().Date;

            IsInitialized = true;

            LaunchDayTimer();
            LaunchWeekTimer();
            LaunchMonthTimer();
        }

        private void LaunchDayTimer()
        {
            _timeToEndDay.Value = GetSecondsUntilEndDay();
            LaunchGlobalTimer(_dayTimer, _timeToEndDay, TimeSpan.FromSeconds(3), null, () =>
            {
                LaunchDayTimer();
                OnNewDay?.Invoke();
            });
        }

        private void LaunchWeekTimer()
        {
            _timeToEndWeek.Value = GetSecondsUntilEndOfWeek();
            LaunchGlobalTimer(_weekTimer, _timeToEndWeek, TimeSpan.FromSeconds(3), null, () =>
            {
                LaunchWeekTimer();
                OnNewWeek?.Invoke();
            });
        }

        private void LaunchMonthTimer()
        {
            _timeToEndMonth.Value = GetSecondsUntilEndOfMonth();
            LaunchGlobalTimer(_monthTimer, _timeToEndMonth, TimeSpan.FromSeconds(3), null, () =>
            {
                LaunchMonthTimer();
                OnNewMonth?.Invoke();
            });
        }

        private float GetSecondsUntilEndDay()
        {
            var now = Now();
            var midnight = now.Date.AddDays(1);
            var timeUntilMidnight = midnight - now;
            return (float)timeUntilMidnight.TotalSeconds;
        }

        private float GetSecondsUntilEndOfWeek()
        {
            var now = Now();
            var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;
            var endOfWeek = now.Date.AddDays(daysUntilSunday + 1);
            var timeUntilEndOfWeek = endOfWeek - now;
            return (float)timeUntilEndOfWeek.TotalSeconds;
        }

        private float GetSecondsUntilEndOfMonth()
        {
            var now = Now();
            var firstDayNextMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
            var timeUntilEndOfMonth = firstDayNextMonth - now;
            return (float)timeUntilEndOfMonth.TotalSeconds;
        }

    public void LaunchLocalTimer(SerialDisposable serialDisposable, ReactiveProperty<float> duration,
            Action completeCallback = null)
        {
            serialDisposable.Disposable = Observable
                .EveryUpdate()
                .Subscribe(onNext: _ =>
                {
                    duration.Value -= Time.deltaTime;
                    if (duration.Value > 0) return;
                    serialDisposable.Disposable?.Dispose();
                    completeCallback?.Invoke();
                });
        }

        public void LaunchLocalCycledTimer(SerialDisposable serialDisposable, ReactiveProperty<float> duration,
            Action callback = null)
        {
            var referenceDuration = duration.Value;
            serialDisposable.Disposable = Observable
                .EveryUpdate()
                .Subscribe(onNext: _ =>
                {
                    duration.Value -= Time.deltaTime;
                    if (duration.Value > 0) return;
                    callback?.Invoke();
                    duration.Value = referenceDuration;
                });
        }

        public void LaunchGlobalTimer(SerialDisposable serialDisposable, ReactiveProperty<float> duration, TimeSpan updatePeriod = default,
            Action<float> updateCallback = null, Action completeCallback = null)
        {
            if(updatePeriod.TotalSeconds < 1)
                updatePeriod = TimeSpan.FromSeconds(1);
            var startTime = Now();
            var lastUpdateTime = startTime;
            var totalDuration = duration.Value;
            
            serialDisposable.Disposable = Observable
                .Interval(updatePeriod)
                .ObserveOnCurrentSynchronizationContext()
                .Subscribe(onNext: _ =>
                {
                    var currentTime = Now();
                    var deltaTime = Mathf.Min(duration.Value, (float)(currentTime - lastUpdateTime).TotalSeconds); 
                    lastUpdateTime = currentTime;
                    var elapsedTime = (float)(currentTime - startTime).TotalSeconds;
                    var remaining = Mathf.Max(0, totalDuration - elapsedTime);
                    duration.Value = remaining;
                    updateCallback?.Invoke(deltaTime);
                    if (remaining > 0) return;
                    serialDisposable.Disposable?.Dispose();
                    completeCallback?.Invoke();
                });
        }

        public void Dispose()
        {
            _dataProvider.OnUpdateData -= OnUpdateDataProvider;
            _dayTimer.Dispose();
            _weekTimer.Dispose();
            _monthTimer.Dispose();
        }

        private void OnUpdateDataProvider() =>
            _data.SaveTime = Now();

        public DateTime Now()
        {
            var elapsed = Time.realtimeSinceStartup - _localStartupTime;
            return _serverStatTime.AddSeconds(elapsed);
        }

        public int DaysFrom(DateTime dateTime) =>
            (Now().Date - dateTime.Date).Days;

        void ITickable.Tick()
        {
            if (_data != null)
                _data.TotalPlaytime += Time.unscaledDeltaTime;
        }

        public void Test_IncrementDays(int amount)
        {
            _serverStatTime = _serverStatTime.AddDays(amount);
            _timeToEndDay.Value -= 60 * 60 * 24 * amount;
            _timeToEndWeek.Value -= 60 * 60 * 24 * amount;
            _timeToEndMonth.Value -= 60 * 60 * 24 * amount;
        }
    }
}