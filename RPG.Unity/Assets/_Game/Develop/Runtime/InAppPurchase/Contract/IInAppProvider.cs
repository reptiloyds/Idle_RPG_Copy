using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Delegates;

namespace PleasantlyGames.RPG.Runtime.InAppPurchase.Contract
{
    public interface IInAppProvider
    {
        event PurchaseDelegate OnPurchaseProcessed;
        IReadOnlyList<string> NonConfirmedPurchases { get; }

        void Initialize();
        decimal GetPrice(string productId);
        string GetCurrency(string productId);
        UniTask<bool> Purchase(string productId);
        UniTask<bool> Confirm(string productId, Action purchaseUsing);
    }
}
