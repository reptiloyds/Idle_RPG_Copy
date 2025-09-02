using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Save;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Model;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Service
{
    public class PiggyBankService : IDisposable
    {
        [Inject] private PiggyBankDataProvider _dataProvider;
        [Inject] private ProductService _productService;
        [Inject] private MainMode _mainMode;
        [Inject] private ResourceService _resource;
        [Inject] private ContentService _content;
        [Inject] private StatImprover _statImprover;
        [Inject] private BalanceContainer _balance;
        [Inject] private ISpriteProvider _sprites;
        [Inject] private IAnalyticsService _analytics;

        private PiggyBankData _data;
        private PiggyBankConstantsSheet.Row _formulaConstants;
        private PiggyBankLevelSheet _levelSheet;
        private List<Product> _products;

        public int Level => _data.Level;
        public int CurrentHard => _data.CurrentHard;

        public event Action OnChanged;
        public event Action OnCollected;

        public void Initialize()
        {
            _data = _dataProvider.GetData();
            _statImprover.OnLevelUpStat += AnyStatImproved;
            _mainMode.OnWin += LevelCompleted;
            _formulaConstants = _balance.Get<PiggyBankConstantsSheet>().FirstOrDefault();
            _levelSheet = _balance.Get<PiggyBankLevelSheet>();
            _products = _productService.Products.Where(p => _levelSheet.Any(x => x.Id == p.Id)).ToList();
        }

        public void Dispose()
        {
            _statImprover.OnLevelUpStat -= AnyStatImproved;
            _mainMode.OnWin -= LevelCompleted;
        }

        private void AnyStatImproved(UnitStatType statType)
        {
            if (!_content.IsUnlocked("PiggyBank_Window") || _data.Level == 0)
                return;

            _data.StatImprovedCount++;
            CalculatePoints();
            OnChanged?.Invoke();
        }

        private void LevelCompleted(IGameMode gameMode)
        {
            if (!_content.IsUnlocked("PiggyBank_Window"))
                return;

            _data.CompletedLevelCount++;
            CalculatePoints();
            OnChanged?.Invoke();
        }

        private void CalculatePoints()
        {
            _data.CurrentHard = (_data.StatImprovedCount / _formulaConstants.Rg) +
                                (_data.CompletedLevelCount * _formulaConstants.Kl);
            if (_data.CurrentHard > GetHardLimit()) _data.CurrentHard = GetHardLimit();
        }

        public async UniTask Collect(Vector3 position)
        {
            var purchaseSuccess = await _productService.Purchase(_products[GetIndex()].Id);
            if (!purchaseSuccess) return;
            
            _resource.AddResource(ResourceType.Hard, _data.CurrentHard, ResourceFXRequest.Create(spawnPosition: position));
            
            if (!IsMaxLevel())
            {
                _analytics.SendPiggyBankBoughtNotMax(_data.Level, _data.Level + 1, _data.CurrentHard, GetHardLimit());
                _data.Level++;
            }
            else
            {
                _analytics.SendPiggyBankBoughtMax(_data.Level - 1, _data.Level, _data.CurrentHard, GetHardLimit());
            }
            
            _data.CurrentHard = 0;
            _data.CompletedLevelCount = 0;
            _data.StatImprovedCount = 0;
            
            OnChanged?.Invoke();
            OnCollected?.Invoke();
        }

        public Sprite GetSprite()
        {
            Sprite sprite = _sprites.GetSprite(_levelSheet[GetIndex()].SpriteId);
            return sprite;
        }

        public int GetHardLimit()
        {
            return _levelSheet[GetIndex()].HardLimit;
        }

        public int GetNextHardLimit()
        {
            return _levelSheet[GetNextIndex()].HardLimit;
        }

        public int GetIndex()
        {
            return Math.Min(_data.Level - 1, _levelSheet.Count - 1);
        }

        public int GetNextIndex()
        {
            return Math.Min(_data.Level - 1, _levelSheet.Count - 1);
        }

        public string GetPrice()
        {
            return _products[GetIndex()].Price.Value.CurrentValue;
        }

        public bool IsMaxLevel()
        {
            return _data.Level - 1 >= _levelSheet.Count - 1;
        }
    }
}