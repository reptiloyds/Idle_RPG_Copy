using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Save;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using PleasantlyGames.RPG.Runtime.SystemFeature.Contract;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Model
{
    public class ProductService
    {
        [Inject] private BalanceContainer _balance;
        [Inject] private IInAppProvider _provider;
        [Inject] private ISaveService _saveService;
        [Inject] private ProductDataProvider _dataProvider;
        [Inject] private ContentService _contentService;
        [Inject] private IObjectResolver _resolver;
        [Inject] private IAnalyticsService _analytics;
        [Inject] private ISystemDataProvider _systemDataProvider;

        private ProductDataContainer _data;

        private readonly Dictionary<string, List<Product>> _mergedProducts = new();
        private readonly List<Product> _products = new();

        public PeriodicProducts PeriodicProducts { get; private set; }
        public IReadOnlyDictionary<string, List<Product>> MergedProducts => _mergedProducts;
        public IReadOnlyList<Product> Products => _products;

        public event Action<Product> OnPurchaseInHandle;

        [Preserve]
        public ProductService()
        {
        }

        public async UniTaskVoid InitializeAsync()
        {
            _data = _dataProvider.GetData();
            CreateProducts();

            PeriodicProducts = new PeriodicProducts(_products, _data);
            _resolver.Inject(PeriodicProducts);
            PeriodicProducts.Initialize();
            await HandleNonConfirmedPurchases();
        }

        public async UniTask<bool> Purchase(string productId)
        {
            var product = GetProduct(productId);
            if (product == null) return false;
            if (product.Access.LimitIsOver) return false;
            if (product.Price.IsFree.CurrentValue)
            {
                HandleProductPurchase(product);
                await _saveService.SaveAndLoadToCloudAsync();
                return true;
            }

            _analytics.SendPurchaseAttempt(product);
            var purchaseSuccess = await _provider.Purchase(product.Id);
            if (!purchaseSuccess) return false;
            return await Confirm(product);
        }

        public Product GetProduct(string productId)
        {
            foreach (var product in _products)
                if (string.Equals(product.Id, productId))
                    return product;

            return null;
        }

        private void CreateProducts()
        {
            var sheet = _balance.Get<ProductsSheet>();
            foreach (var data in _data.List)
            {
                if (!sheet.TryGetValue(data.Id, out var config)) continue;

                var content = _contentService.GetProduct(config.Id);

                var product = new Product(config, data, content);
                _resolver.Inject(product);
                _products.Add(product);
                product.Initialize();
                string mergeKey = product.MergedKey;
                if (string.IsNullOrEmpty(mergeKey)) continue;
                if (_mergedProducts.TryGetValue(mergeKey, out var products))
                    products.Add(product);
                else
                    _mergedProducts.Add(mergeKey, new List<Product> { product });
            }
        }

        private async UniTask HandleNonConfirmedPurchases()
        {
            var purchases = _provider.NonConfirmedPurchases;

            var purchasesCount = purchases.Count;
            for (var i = 0; i < purchasesCount; i++)
            {
                var model = GetProduct(purchases[0]);
                if (model == null) continue;
                await Confirm(model);
            }
        }

        private async UniTask<bool> Confirm(Product product)
        {
            var success = await _provider.Confirm(product.Id, () => HandleProductPurchase(product));
            if (success)
                await _saveService.SaveAndLoadToCloudAsync();

            _analytics.SendIfFirstPurchase(product);

            _analytics.SendCompletedPurchase(
                _systemDataProvider.PlatformId,
                product,
                _provider.GetPrice(product.Id),
                _provider.GetCurrency(product.Id));

            OnPurchaseInHandle?.Invoke(product);
            return success;
        }

        private void HandleProductPurchase(Product product)
        {
            OnPurchaseInHandle?.Invoke(product);
            product.HandlePurchase();
        }
    }
}