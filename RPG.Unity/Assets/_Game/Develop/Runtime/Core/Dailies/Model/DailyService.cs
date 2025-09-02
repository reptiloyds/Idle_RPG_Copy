using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.BuyItem;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.ClearDungeon;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.ClearLevel;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.CompleteDailies;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.DefeatEnemy;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.SpinRoulette;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.WatchAd;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Save;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Type;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using R3;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model
{
    public class DailyService : IDisposable
    {
        [Inject] private DailyDataProvider _dataProvider;
        [Inject] private BalanceContainer _balance;
        [Inject] private ITranslator _translator;
        [Inject] private ResourceService _resourceService;
        [Inject] private MainMode _mainMode;
        [Inject] private DungeonModeFacade _dungeonFacade;
        [Inject] private LootboxService _lootboxService;
        [Inject] private IAdService _adService;
        [Inject] private RouletteFacade _rouletteFacade;
        [Inject] private TimeService _timeService;

        private const int _maxViewReward = 5;
        private DailyDataContainer _data;
        private readonly List<Daily> _dailies = new();

        public ReadOnlyReactiveProperty<float> DelayToReset => _timeService.TimeToEndDay;
        public IReadOnlyList<Daily> Dailies => _dailies;

        public event Action OnResetProgress;
        public event Action<Daily> OnDailyCompleted;
        public event Action<Daily> OnDailyCollected;
        
        [Preserve]
        public DailyService()
        {
        }

        public void Initialize()
        {
            _data = _dataProvider.GetData();
            CreateModels();
            if (_timeService.IsFirstSessionToday)
                ResetProgress();

            _timeService.OnNewDay += ResetProgress;
        }

        public void Dispose()
        {
            foreach (var daily in _dailies)
                daily.OnCompleted -= OnCompleted;
            
            _timeService.OnNewDay -= ResetProgress;
        }

        private void ResetProgress()
        {
            foreach (var daily in Dailies) 
                daily.ResetProgress();
            OnResetProgress?.Invoke();
        }

        public void CollectReward(Daily daily, Vector3 position)
        {
            _resourceService.AddResource(daily.RewardType,
                daily.RewardAmount, ResourceFXRequest.Create(spawnPosition: position, maxViewReward: _maxViewReward));
            
            daily.CollectReward();
            OnDailyCollected?.Invoke(daily);
        }

        private void CreateModels()
        {
            var sheet = _balance.Get<DailiesSheet>();
            foreach (var kvp in _data.DataDictionary)
                _dailies.Add(CreateDaily(sheet[kvp.Key], kvp.Value));
        }

        private Daily CreateDaily(DailyRow config, DailyData data)
        {
            Daily daily = null;
            switch (config.Type)
            {
                case DailyType.DefeatEnemy:
                    daily = new DefeatEnemyDaily(config, data, _resourceService, _translator, _mainMode);
                    break;
                case DailyType.ClearLevel:
                    daily = new ClearLevelDaily(config, data, _resourceService, _translator, _mainMode);
                    break;
                case DailyType.ClearDungeon:
                    daily = new ClearDungeonDaily(config, data, _resourceService, _translator, _dungeonFacade);
                    break;
                case DailyType.WatchAd:
                    daily = new WatchAdDaily(config, data, _resourceService, _translator, _adService);
                    break;
                case DailyType.BuyItem:
                    daily = new BuyItemDaily(config, data, _resourceService, _translator, _lootboxService);
                    break;
                case DailyType.CompleteDailies:
                    daily = new CompleteDailiesDaily(config, data, _resourceService, _translator, this);
                    break;
                case DailyType.SpinRoulette:
                    daily = new SpinRouletteDaily(config, data, _resourceService, _translator, _rouletteFacade);
                    break;
            }

            if (daily != null)
            {
                daily.OnCompleted += OnCompleted;
                daily.Initialize();
            }

            return daily;
        }

        private void OnCompleted(Daily daily) =>
            OnDailyCompleted?.Invoke(daily);
    }
}