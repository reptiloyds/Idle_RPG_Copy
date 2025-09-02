using System;
using PleasantlyGames.RPG.Runtime.Analytics.Types;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Model.Periodic;
using PleasantlyGames.RPG.Runtime.Core.Products.Save;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using Sirenix.OdinInspector;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Model
{
    public class Product : IDisposable
    {
        [Inject] private TimeService _time;
        [Inject] private ITranslator _translator;
        [Inject] private IObjectResolver _resolver;

        private readonly ProductRow _config;
        private readonly ProductData _data;
        
        [ShowInInspector, ReadOnly]
        public string Id => _config.Id;
        
        public ProductPlacement Placement => _config.Placement;
        public PeriodicProductData PeriodicData => _config.PeriodicData;
        public ProductAccess Access { get; }
        public ProductPrice Price { get; private set; }
        public ProductRewards Rewards { get; private set; }
        public ProductVisual Visual { get; private set; }
        
        public bool IsMerged { get; private set; }
        public string MergedKey { get; private set; }
        public AnalyticsItemType AnalyticsItemType => _config.AnalyticsItemType;

        public event Action<Product> OnRefreshed;
        public event Action<Product> OnPurchaseHandled;

        public Product(ProductRow config,
            ProductData data,
            IUnlockable unlockable = null)
        {
            _config = config;
            _data = data;
            Access = new ProductAccess(config, data, new AllUnlockable(unlockable), this);
        }

        [Inject]
        private void Construct()
        {
            Price = new ProductPrice(_config, _data);
            _resolver.Inject(Price);
            Rewards = new ProductRewards(_config, _data);
            _resolver.Inject(Rewards);
            Visual = new ProductVisual(_config, Rewards);
            _resolver.Inject(Visual);
        }
        
        public void Initialize()
        {
            Rewards.Initialize();
            foreach (var reward in Rewards.List)
            {
                Access.AddUnlockable(reward);
                if (!string.IsNullOrEmpty(MergedKey) || string.IsNullOrEmpty(reward.MergeKey)) continue;
                IsMerged = true;
                MergedKey = reward.MergeKey;
            }
            Visual.Initialize();
            Access.Initialize();
        }

        public void HandlePurchase()
        {
            _data.LastPurchase = _time.Now();
            Rewards.ApplyRewards();
            Access.IncrementPurchasesAmount();
            Price.AfterPurchase();
            OnPurchaseHandled?.Invoke(this);
        }

        public string GetName()
        {
            var localizedName = _translator.Translate(_config.NameLocToken);
            if (!Access.HasLimit) return localizedName;
            return $"{localizedName} ({_data.LimitCounter}/{_config.PurchaseLimit})";
        }

        public void Refresh()
        {
            _data.BonusCollected = false;
            _data.LimitCounter = 0;
            OnRefreshed?.Invoke(this);
        }

        public void Dispose() => 
            Access.Dispose();
    }
}