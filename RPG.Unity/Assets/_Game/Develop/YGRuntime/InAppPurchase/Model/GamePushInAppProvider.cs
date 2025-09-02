using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamePush;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Delegates;
using UnityEngine;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.YGRuntime.InAppPurchase.Model
{
    public class GamePushInAppProvider : IInAppProvider
    {
        private List<string> _notConsumedPurchases;
        
        private List<FetchProducts> _products;
        
        private UniTaskCompletionSource _fetchProductTcs;
        private UniTaskCompletionSource _fetchPlayerPurchasesTcs;

        private UniTaskCompletionSource<bool> _purchaseCompletionSource;
        private UniTaskCompletionSource<bool> _consumeCompletionSource;

        public event PurchaseDelegate OnPurchaseProcessed;

        public IReadOnlyList<string> NonConfirmedPurchases => _notConsumedPurchases;

        [UnityEngine.Scripting.Preserve]
        public GamePushInAppProvider() { }

        public void Initialize()
        {
        }

        public void SetupProducts(List<FetchProducts> products) => 
            _products = products;

        public void SetupNonPurchased(List<string> nonConsumedPurchases) => 
            _notConsumedPurchases = nonConsumedPurchases;

        public decimal GetPrice(string productId)
        {
            var product = GetProduct(productId);
            if (product == null) return -1;

            return product.price;
        }

        public string GetCurrency(string productId)
        {
            var product = GetProduct(productId);
            if (product == null) return string.Empty;

            return product.currency;
        }

        private FetchProducts GetProduct(string productId)
        {
            if (_products == null) return null;
            foreach (var product in _products)
                if (string.Equals(product.tag, productId))
                    return product;

            return null;
        }

        public async UniTask<bool> Purchase(string productId)
        {
            _purchaseCompletionSource = new UniTaskCompletionSource<bool>();
            GP_Payments.Purchase(productId, OnPurchaseSuccess, OnPurchaseError);
            return await _purchaseCompletionSource.Task;
        }

        private void OnPurchaseSuccess(string productId)
        {
            OnPurchaseProcessed?.Invoke(productId, true);
            _purchaseCompletionSource.TrySetResult(true);
        }

        private void OnPurchaseError()
        {
            OnPurchaseProcessed?.Invoke("Unknown", false);
            _purchaseCompletionSource.TrySetResult(false);
        }

        public async UniTask<bool> Confirm(string productId, Action purchaseUsing)
        {
            _consumeCompletionSource = new UniTaskCompletionSource<bool>();
            GP_Payments.Consume(productId, OnConsumeSuccess, OnConsumeError);
            var result = await _consumeCompletionSource.Task;
            if(result)
                purchaseUsing.Invoke();
            return result;
        }

        private void OnConsumeSuccess(string productId)
        {
            _consumeCompletionSource.TrySetResult(true);
        }

        private void OnConsumeError()
        {
            Logger.LogWarning($"ErrorConsume");
            _consumeCompletionSource.TrySetResult(false);
        }
    }
}