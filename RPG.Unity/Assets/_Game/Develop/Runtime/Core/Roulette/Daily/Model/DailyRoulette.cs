using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Contract;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Definition;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Save;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.RouletteWheel;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using R3;
using UnityEngine;
using VContainer;
using Observable = R3.Observable;

namespace PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model
{
    public class DailyRoulette : IRoulette, IDisposable
    {
        private DailyRouletteData _data;
        private RouletteSheet.Row _config;
        private DailyRouletteDefinition _definition;

        private readonly List<WheelPieceData> _pieceDataList = new();
        private readonly Dictionary<int, RouletteSheet.Elem> _pieceDictionary = new();
        private readonly SerialDisposable _cooldownDisposable = new();
        private readonly CompositeDisposable _compositeDisposable = new();
        
        [Inject] private ResourceService _resourceService;
        [Inject] private TimeService _timeService;
        [Inject] private BalanceContainer _balance;

        private readonly ReactiveProperty<float> _cooldown = new();
        private ReactiveProperty<int> _freeSpinAmount;
        private ReactiveProperty<int> _spinAmount;
        
        public List<WheelPieceData> PieceDataList => _pieceDataList;
        public IReadOnlyDictionary<int, RouletteSheet.Elem> PieceDictionary => _pieceDictionary;

        public RouletteType Type => RouletteType.DailyWheel;
        public event Action OnSpin;

        public int BonusSpinAmount => _definition.SpinAmount;
        public int TotalSpinAmount => _freeSpinAmount.CurrentValue + _spinAmount.CurrentValue;
        public ReadOnlyReactiveProperty<int> FreeSpinAmount => _freeSpinAmount;
        public ReadOnlyReactiveProperty<int> SpinAmount => _spinAmount;
        public ReadOnlyReactiveProperty<float> Cooldown => _cooldown;
        public ReadOnlyReactiveProperty<bool> IsCooldown { get; private set; }

        [Preserve]
        public DailyRoulette()
        {
        }
        
        public void Setup(DailyRouletteData data) => 
            _data = data;

        public void Initialize()
        {
            IsCooldown = _cooldown.Select(value => value > 0).ToReadOnlyReactiveProperty();
            _spinAmount = new ReactiveProperty<int>(_data.SpinAmount);
            _spinAmount
                .Subscribe(value => _data.SpinAmount = value)
                .AddTo(_compositeDisposable);
            
            _freeSpinAmount = new ReactiveProperty<int>(_data.FreeSpinAmount);
            _freeSpinAmount
                .Subscribe(value => _data.FreeSpinAmount = value)
                .AddTo(_compositeDisposable);
            
            var rouletteSheet = _balance.Get<RouletteSheet>();
            rouletteSheet.TryGetValue(RouletteType.DailyWheel, out _config);
            _definition = JsonConvertLog.DeserializeObject<DailyRouletteDefinition>(_config.AdditionalData);
            
            var now = _timeService.Now();
            var lastSpinSpan = now - _data.LastSpinTime;

            if (_timeService.IsFirstSessionToday) 
                RefreshSpinAmount();
            _timeService.OnNewDay += RefreshSpinAmount;

            if (lastSpinSpan.TotalSeconds < _definition.Cooldown) 
                StartCooldown();
            

            for (int i = 0; i < _config.Count; i++)
            {
                var elem = _config[i];
                var resource = _resourceService.GetResource(elem.ResourceType);
                _pieceDataList.Add(new WheelPieceData()
                {
                    Amount = elem.Amount,
                    Sprite = resource.Sprite,
                    Weight = elem.Weight,
                    Index = i,
                });
                _pieceDictionary.Add(i, elem);
            }
        }

        public void Dispose()
        {
            _timeService.OnNewDay -= RefreshSpinAmount;
            _compositeDisposable.Dispose();
            _cooldownDisposable.Dispose();
        }

        private void RefreshSpinAmount()
        {
            _spinAmount.Value = _definition.SpinAmount;
            _freeSpinAmount.Value = _definition.FreeSpinAmount;
        }

        public void Spin()
        {
            if (TotalSpinAmount == 0)
                return;
            
            _data.LastSpinTime = _timeService.Now();
            if (_freeSpinAmount.Value > 0)
                _freeSpinAmount.Value--;
            else
                _spinAmount.Value--;
            
            if(TotalSpinAmount > 0)
                StartCooldown();
            
            OnSpin?.Invoke();
        }

        private void StartCooldown()
        {
            if(IsCooldown.CurrentValue) return;
            var now = _timeService.Now();
            var lastSpinSpan = now - _data.LastSpinTime;
            
            if(lastSpinSpan.TotalSeconds >= _definition.Cooldown) return;

            _cooldown.Value = _definition.Cooldown - (float)lastSpinSpan.TotalSeconds;
            
            _timeService.LaunchGlobalTimer(_cooldownDisposable, _cooldown, TimeSpan.FromSeconds(1), completeCallback: StopCooldown);
        }

        private void StopCooldown() => 
            _cooldownDisposable.Disposable?.Dispose();

        public void ApplyReward(int index, RectTransform source)
        {
            var pieceConfig = _pieceDictionary[index];
            _resourceService.AddResource(pieceConfig.ResourceType, pieceConfig.Amount,
                ResourceFXRequest.Create(spawnPosition: source.position));
        }
    }
}