using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.VIP.Save;
using PleasantlyGames.RPG.Runtime.VIP.Sheets;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.VIP.Model
{
    public class VipService : IDisposable
    {
        [Inject] private VipDataProvider _dataProvider;
        [Inject] private ProductService _productService;
        [Inject] private TimeService _timeService;
        [Inject] private PassiveIncomeModel _passiveIncome;
        [Inject] private BalanceContainer _balance;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private ITranslator _translator;
        [Inject] private DailyRoulette _dailyRoulette;

        private const int DURATION_IN_DAYS = 30;

        private const string PRODUCT_ID = "VIP_status";

        private const int PASSIVE_INCOME_HOURS = 24;

        private int _passiveIncomeHours => PASSIVE_INCOME_HOURS;
        private VipDataContainer _data;
        private DateTime _expirationDate;
        private VipPurchaseReward[] _purchaseRewards;
        private readonly SerialDisposable _expirationTimerDisposable = new();
        private readonly ReactiveProperty<float> _duration = new();
        private readonly ReactiveProperty<bool> _isActive = new(false);
        private readonly ReactiveProperty<bool> _extensionRequired = new(false);
        private VipBonusData[] _bonusData;

        public Product Product { get; private set; }
        public IReadOnlyCollection<VipBonusData> BonusData => _bonusData;
        public IReadOnlyCollection<VipPurchaseReward> PurchaseRewards => _purchaseRewards;
        public ReadOnlyReactiveProperty<bool> IsActive => _isActive;
        public ReadOnlyReactiveProperty<float> Duration => _duration;
        public ReadOnlyReactiveProperty<bool> ExtensionRequired => _extensionRequired; 
        public int DurationInDays => DURATION_IN_DAYS;
        public event Action OnActivate;

        public void Initialize()
        {
            _data = _dataProvider.GetData();
            Product = _productService.GetProduct(PRODUCT_ID);
            _purchaseRewards = new VipPurchaseReward[Product.Rewards.List.Count];
            int index = 0;
            foreach (var reward in Product.Rewards.List) 
                _purchaseRewards[index++] = new VipPurchaseReward(reward.BackColor, reward.Sprite, reward.Name);

            UpdateExpirationDate();

            if (IsExpired())
            {
                if (_data.ShouldOfferExtension)
                    _extensionRequired.Value = true;
            }
            else
                RestoreActiveStatus();

            _timeService.OnNewDay += OnNewDay;
            CreateBonusData();
        }

        public async UniTask<bool> TryActivate()
        {
            if (_isActive.CurrentValue) return false;
            var purchaseSuccess = await _productService.Purchase(PRODUCT_ID);
            if (!purchaseSuccess) return false;
            Activate();
            return true;
        }

        public string GetRouletteDefinitions() => 
            String.Format(_translator.Translate("vip_roulette_def"), _dailyRoulette.BonusSpinAmount);

        public string GetLabel() => 
            String.Format(_translator.Translate("vip_label"), DurationInDays);
        
        public void OnExtensionOffered()
        {
            _data.ShouldOfferExtension = false;
            _extensionRequired.Value = false;
        }

        private void CreateBonusData()
        {
            var sheet = _balance.Get<VipBonusViewSheet>();
            _bonusData = new VipBonusData[sheet.Count];
            int index = 0;
            foreach (var row in sheet)
            {
                var sprite = _spriteProvider.GetSprite(row.Sprite);
                var label = _translator.Translate(row.LabelToken);
                var definition = _translator.Translate(row.DefinitionToken);
                _bonusData[index++] = new VipBonusData(sprite, label, definition);
            }
        }

        private void Activate()
        {
            _data.ActivationDate = _timeService.Now();
            _data.ActivationAmount++;
            _data.ShouldOfferExtension = true;
            UpdateExpirationDate();
            _isActive.Value = true;
            ApplyDailyBonuses();
            ApplyPermanentBonuses();
            OnActivate?.Invoke();
        }

        private void Deactivate()
        {
            _isActive.Value = false;
            DisablePermanentBonuses();
            _extensionRequired.Value = true;
        }

        private void RestoreActiveStatus()
        {
            _isActive.Value = true;
            if (_timeService.IsFirstSessionToday)
                ApplyDailyBonuses();
            ApplyPermanentBonuses();
        }

        private bool IsExpired() =>
            _timeService.Now() >= _expirationDate;

        private void UpdateExpirationDate()
        {
            _expirationTimerDisposable.Disposable?.Dispose();
            _expirationDate = _data.ActivationDate.AddDays(DurationInDays);
            var expirationSpan = _expirationDate - _timeService.Now();
            if (expirationSpan <= TimeSpan.Zero) return;

            _duration.Value = (float)expirationSpan.TotalSeconds;
            _timeService.LaunchGlobalTimer(_expirationTimerDisposable, _duration, TimeSpan.FromMinutes(1),
                completeCallback: OnExpirationTimerComplete);
        }

        private void OnExpirationTimerComplete()
        {
            if (!IsActive.CurrentValue) return;
            Deactivate();
        }

        private void OnNewDay()
        {
            if (!_isActive.CurrentValue) return;
            ApplyDailyBonuses();
        }

        private void ApplyDailyBonuses()
        {
            //TODO post bonuses
        }

        private void ApplyPermanentBonuses() =>
            _passiveIncome.SetIncomeLimitInHours(_passiveIncomeHours);

        private void DisablePermanentBonuses() =>
            _passiveIncome.SetDefaultIncomeLimit();

        void IDisposable.Dispose()
        {
            _expirationTimerDisposable.Dispose();
            _timeService.OnNewDay -= OnNewDay;
        }
    }
}