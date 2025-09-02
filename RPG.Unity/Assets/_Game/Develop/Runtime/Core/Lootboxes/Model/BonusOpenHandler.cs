using System;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Save;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using R3;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model
{
    public class BonusOpenHandler : IDisposable
    {
        private readonly TimeService _time;
        private readonly LootboxData _data;
        private readonly LootboxBonusOpenData _config;

        private readonly SerialDisposable _cooldownTimer = new();
        private readonly ReactiveProperty<float> _cooldown = new();
        private ReactiveProperty<int> _openAmount;
        
        public int Items => Math.Min(_config.StartAmount + _data.BonusOpenCounter, _config.EndAmount);
        public int MaxItems => _config.EndAmount;
        public int MaxOpenAmount => _config.OpenAmount; 
        public ReadOnlyReactiveProperty<int> OpenAmount => _openAmount;
        public ReadOnlyReactiveProperty<float> Cooldown => _cooldown;
        public ReadOnlyReactiveProperty<bool> IsCooldown { get; private set; }

        public event Action<int> OnProcessed;

        public BonusOpenHandler(LootboxData data, LootboxBonusOpenData config, TimeService timeService)
        {
            _data = data;
            _config = config;
            _time = timeService;
        }

        public void Initialize()
        {
            IsCooldown = _cooldown.Select(value => value > 0).ToReadOnlyReactiveProperty();
            _openAmount = new ReactiveProperty<int>(_data.BonusOpenAmount);
            _openAmount.Subscribe(value => _data.BonusOpenAmount = value);
            if (_time.IsFirstSessionToday)
                RefreshBonusOpenAmount();
            _time.OnNewDay += RefreshBonusOpenAmount;
            
            var secondSinceLastAd = (int)(_time.Now() - _data.LastBonusTime).TotalSeconds;
            var hasCooldown = secondSinceLastAd > 0 && secondSinceLastAd < _config.Cooldown;
            if (hasCooldown && _openAmount.Value > 0) 
                StartCooldown(_config.Cooldown - secondSinceLastAd);
        }

        private void RefreshBonusOpenAmount() => 
            _openAmount.Value = _config.OpenAmount;

        public void HandleReward()
        {
            var rewardAmount = Items;
            _data.LastBonusTime = _time.Now();
            _data.BonusOpenCounter++;
            _openAmount.Value--;
            if(_openAmount.Value > 0)
                StartCooldown(_config.Cooldown);
            OnProcessed?.Invoke(rewardAmount);
        }

        private void StartCooldown(float duration)
        {
            _cooldown.Value = duration;
            _time.LaunchGlobalTimer(_cooldownTimer, _cooldown, TimeSpan.FromSeconds(1));
        }

        public void Dispose() => 
            _time.OnNewDay -= RefreshBonusOpenAmount;
    }
}