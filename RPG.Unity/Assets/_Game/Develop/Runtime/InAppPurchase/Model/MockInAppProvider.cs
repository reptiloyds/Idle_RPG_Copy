using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Delegates;

namespace PleasantlyGames.RPG.Runtime.InAppPurchase.Model
{
    public class MockInAppProvider : IInAppProvider
    {
        public readonly List<string> _nonConfirmedProducts = new();
        
        public event PurchaseDelegate OnPurchaseProcessed;
        public event ConsumeDelegate OnConsumeProcessed;
        public IReadOnlyList<string> NonConfirmedPurchases => _nonConfirmedProducts;

        public void Initialize()
        {
        }

        decimal IInAppProvider.GetPrice(string productId) => 
            1;

        string IInAppProvider.GetCurrency(string productId) => 
            "MOCK";

        UniTask<bool> IInAppProvider.Purchase(string productId)
        {
            OnPurchaseProcessed?.Invoke(productId, true);
            return UniTask.FromResult(true);
        }

        UniTask<bool> IInAppProvider.Confirm(string productId, Action purchaseUsing)
        {
            purchaseUsing?.Invoke();
            OnConsumeProcessed?.Invoke(productId, true);
            return UniTask.FromResult(true);
        }
    }
}