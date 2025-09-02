using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Delegates;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using VContainer;

namespace PleasantlyGames.RPG.AndroidRuntime.InApp.Model
{
    public class UnityPurchaseProvider : IInAppProvider, IDetailedStoreListener
    {
        [Inject] private UnityPurchaseDataProvider _dataProvider;
        private UniTaskCompletionSource<bool> _purchaseCompletionSource;
        
        private IStoreController _storeController;
        private UnityPurchaseDataContainer _data;

        public IReadOnlyList<string> NonConfirmedPurchases => _data.NonConfirmedPurchases;
        
        public event PurchaseDelegate OnPurchaseProcessed;
        public event Action OnInitialized;
        public event Action<InitializationFailureReason, string> OnInitializedFailed;
        
        void IInAppProvider.Initialize()
        {
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _data = _dataProvider.GetData();
            _storeController = controller;
            OnInitialized?.Invoke();
        }
        
        void IStoreListener.OnInitializeFailed(InitializationFailureReason error) => 
            OnInitializedFailed?.Invoke(error, string.Empty);

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message) => 
            OnInitializedFailed?.Invoke(error, message);

        decimal IInAppProvider.GetPrice(string productId)
        {
            if (_storeController == null) return 0;
            var product = _storeController.products.WithID(productId);
            return product.metadata.localizedPrice;
        }
        
        string IInAppProvider.GetCurrency(string productId)
        {
            if (_storeController == null) return "NULL";
            var product = _storeController.products.WithID(productId);
            return product.metadata.isoCurrencyCode;
        }
        
        UniTask<bool> IInAppProvider.Purchase(string productId)
        {
            if (_storeController == null) return UniTask.FromResult(true);
            _purchaseCompletionSource = new UniTaskCompletionSource<bool>();
            _storeController.InitiatePurchase(productId);
            return _purchaseCompletionSource.Task;
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var productId = purchaseEvent.purchasedProduct.definition.id;
            var productType = purchaseEvent.purchasedProduct.definition.type;
            if (productType != ProductType.Consumable && _data.RecoverableProducts.Contains(productId))
                return PurchaseProcessingResult.Complete;
            
            if (!_data.NonConfirmedPurchases.Contains(productId))
            {
                _data.NonConfirmedPurchases.Add(productId);
                OnPurchaseProcessed?.Invoke(purchaseEvent.purchasedProduct.definition.id, true);
                _purchaseCompletionSource?.TrySetResult(true);
            }
            return PurchaseProcessingResult.Pending;
        }

        UniTask<bool> IInAppProvider.Confirm(string productId, Action purchaseUsing)
        {
            if (!_data.NonConfirmedPurchases.Contains(productId)) return UniTask.FromResult(false);
            if (_storeController == null) return UniTask.FromResult(true); //For editor test
            var product = _storeController.products.WithID(productId);
            if (product == null) return UniTask.FromResult(false);

            var productType = product.definition.type;
            if (productType != ProductType.Consumable) 
                _data.RecoverableProducts.Add(productId);
            
            _data.NonConfirmedPurchases.Remove(productId);
            purchaseUsing.Invoke();
            _storeController.ConfirmPendingPurchase(product);
            
            return UniTask.FromResult(true);
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            OnPurchaseProcessed?.Invoke(product.definition.id, false);
            _purchaseCompletionSource.TrySetResult(false);
        }
        
        void IDetailedStoreListener.OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            OnPurchaseProcessed?.Invoke(product.definition.id, false);
            _purchaseCompletionSource.TrySetResult(false);
        }
    }
}