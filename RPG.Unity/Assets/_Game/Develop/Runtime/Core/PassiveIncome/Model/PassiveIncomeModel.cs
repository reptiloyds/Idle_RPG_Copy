using System;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Definition;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Save;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using R3;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Model
{
    public class PassiveIncomeModel : IDisposable
    {
        [Inject] private PassiveIncomeDataProvider _dataProvider;
        [Inject] private PassiveIncomeConfiguration _configuration;
        [Inject] private TimeService _time;
        [Inject] private ITranslator _translator;
        [Inject] private ResourceService _resourceService;
        [Inject] private MainMode _mainMode;
        [Inject] private UnitStatService _statService;
        
        private PassiveIncomeDataContainer _data;
        private TimeSpan _incomeTimeLimit;
        private bool _incomeLimitInitialized;
        
        private int _lastWaveLevel;
        private BigDouble.Runtime.BigDouble _softPerTick;

        private readonly SerialDisposable _rewardTimer = new();
        private readonly ReactiveProperty<float> _rewardDuration = new();
        
        private readonly SerialDisposable _cooldownTimer = new();
        private readonly ReactiveProperty<float> _cooldownDuration = new();
        private readonly ReactiveProperty<BigDouble.Runtime.BigDouble> _softIncome = new();

        public float BonusIncomeK => _configuration.BonusIncomeK;

        public ReadOnlyReactiveProperty<BigDouble.Runtime.BigDouble> SoftIncome => _softIncome; 
        public ReadOnlyReactiveProperty<float> Cooldown => _cooldownDuration;
        public ReadOnlyReactiveProperty<bool> IsCooldown { get; private set; }
        
        public bool IsRewardReady { get; private set; }

        [Preserve]
        public PassiveIncomeModel()
        {
        }
        
        public void Initialize()
        {
            IsCooldown = _cooldownDuration.Select(value => value > 0).ToReadOnlyReactiveProperty();
            
            _data = _dataProvider.GetData();
            if(!_incomeLimitInitialized)
                SetDefaultIncomeLimit();
            else
            {
                if (_data.TotalMinutes < _incomeTimeLimit.TotalMinutes) 
                    StartIncome();
            }
            CalculateOfflineIncome();
        }

        public void SetDefaultIncomeLimit() => 
            SetIncomeLimitInHours(_configuration.IncomeHoursLimit);

        public void SetIncomeLimitInHours(int incomeHoursLimit)
        {
            _incomeLimitInitialized = true;
            _incomeTimeLimit = TimeSpan.FromHours(incomeHoursLimit);
            if(_data == null) return;
            if (_data.TotalMinutes < _incomeTimeLimit.TotalMinutes) 
                StartIncome();
        }

        public string GetFormatedIncomeTime() => 
            string.Format(_translator.Translate(TranslationConst.PassiveIncomeTime), _data.TotalMinutes);

        public string GetFormatedOnlineIncomeSpeed() => 
            $"{StringExtension.Instance.CutBigDouble(GetSoftPerTick() * _configuration.OnlineSoftK)}/{_translator.Translate(TranslationConst.ShortMinute)}";

        public void CollectBonusIncome(Vector3 position)
        {
            _softIncome.Value *= BonusIncomeK;
            CollectIncome(position);
        }

        public void CollectIncome(Vector3 position)
        {
            _resourceService.AddResource(ResourceType.Soft, _softIncome.Value,
                ResourceFXRequest.Create(spawnPosition: position, maxViewReward: 6, context: PopupIconContext.OverUI));
            _data.TotalMinutes = 0;
            _softIncome.Value = 0;

            StartIncome();
            
            if (_configuration.CooldownInMinutes > 0) 
                StartCooldown();
        }

        private void StartIncome()
        {
            _rewardTimer.Disposable?.Dispose();
            _rewardDuration.Value = (float)_incomeTimeLimit.TotalSeconds;
            _time.LaunchGlobalTimer(_rewardTimer, _rewardDuration, TimeSpan.FromMinutes(_configuration.TickDurationInMinutes), IncomeTick);
        }

        private void StartCooldown()
        {
            _cooldownDuration.Value = _configuration.CooldownInMinutes * 60;
            _time.LaunchGlobalTimer(_cooldownTimer, _cooldownDuration, TimeSpan.FromSeconds(1), completeCallback: CompleteCooldown);
        }

        private void CompleteCooldown() => 
            _cooldownTimer.Disposable?.Dispose();

        private void CalculateOfflineIncome()
        {
            var offlineMinutes = GetOfflineMinutes();
            var tickAmount = offlineMinutes / _configuration.TickDurationInMinutes;
            
            _data.LastTickTime = _time.Now();
            _data.TotalMinutes += offlineMinutes;
            _softIncome.Value += tickAmount * GetSoftPerTick();

            if (tickAmount > 0 || _data.TotalMinutes > _configuration.CooldownInMinutes)
                IsRewardReady = true;
        }
        
        private void IncomeTick(float deltaTime)
        {
            var tickAmount = 1;
            if (deltaTime > 60) 
                tickAmount = Mathf.FloorToInt(deltaTime / 60);
            _data.LastTickTime = _time.Now();
            _data.TotalMinutes += _configuration.TickDurationInMinutes * tickAmount;
            _softIncome.Value += GetSoftPerTick() * tickAmount;
        }

        private BigDouble.Runtime.BigDouble GetSoftPerTick()
        {
            if (_lastWaveLevel == _mainMode.Level) return _softPerTick;
            
            _lastWaveLevel = _mainMode.Level;
            int enemyAmount = 0;
            BigDouble.Runtime.BigDouble totalReward = 0;
            var enemyLevel = _mainMode.CalculateEnemyLevel();
            foreach (var wave in _mainMode.Waves)
            {
                foreach (var enemyData in wave)
                {
                    enemyAmount++;
                    totalReward += _statService.GetValue(enemyData.UnitId, UnitStatType.Reward, enemyLevel);
                }
            }
                
            _softPerTick = BigDouble.Runtime.BigDouble.Ceiling(totalReward / enemyAmount);

            return _softPerTick;
        }

        private int GetOfflineMinutes()
        {
            TimeSpan offline = _time.Now() - _data.LastTickTime;
            var possibleIncomeMinutes = _incomeTimeLimit.TotalMinutes - _data.TotalMinutes;
            if (offline.TotalMinutes < possibleIncomeMinutes)
                return (int)offline.TotalMinutes;

            return (int)possibleIncomeMinutes;
        }

        public void Dispose()
        {
            _rewardTimer.Dispose();
            _cooldownTimer.Dispose();
        }
    }
}