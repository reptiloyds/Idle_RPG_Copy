using System;
using System.Collections.Generic;
using _Game.Scripts.Systems.Nutaku;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.NutakuRuntime.Nutaku;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Delegates;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.Model;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.View;
using VContainer;

namespace PleasantlyGames.RPG.NutakuRuntime.InApp.Model
{
    public class NutakuInAppProvider : IInAppProvider
    {
        [Inject] private BalanceContainer _balance;
        [Inject] private NutakuSystem _nutakuSystem;
        [Inject] private ITranslator _translator;
        [Inject] private TechnicalMessageService _technicalMessageService;
        
        private UniTaskCompletionSource<bool> _purchaseCompletionSource;

        private NutakuInApp _lastInApp;
        private ProductsSheet _sheet;
        private const string MockCurrency = "MOCK"; 
        private const int MockPrice = -1;
        private readonly List<string> _nonConfirmedPurchases = new(0);
        
        public event PurchaseDelegate OnPurchaseProcessed;

        public IReadOnlyList<string> NonConfirmedPurchases => _nonConfirmedPurchases;

        [UnityEngine.Scripting.Preserve]
        public NutakuInAppProvider()
        {
        }
        
        public void Initialize()
        {
            _sheet = _balance.Get<ProductsSheet>();
            _technicalMessageService.GetAsync<ProcessPurchaseView>().Forget();
        }

        public decimal GetPrice(string productId)
        {
            var product = GetProductConfig(productId);
            if (product == null) return MockPrice;
            var price = product.LocalPrice;
            return price <= 0 ? MockPrice : product.LocalPrice;
        }

        public string GetCurrency(string productId)
        {
            var product = GetProductConfig(productId);
            if (product == null) return MockCurrency;
            var currency = product.LocalCurrency;
            return string.IsNullOrEmpty(currency) ? MockCurrency : product.LocalCurrency;
        }

        public async UniTask<bool> Purchase(string productId)
        {
            _purchaseCompletionSource = new UniTaskCompletionSource<bool>();
            var product = GetProductConfig(productId);
            if (product == null) return false;
            _lastInApp = new NutakuInApp()
            {
                Id = product.NutakuId.ToString(),
                FormattedName = _translator.Translate(product.NameLocToken),
                FormattedDescription = _translator.Translate(product.DescriptionLocToken),
                ImageUrl = product.ImageUrl,
                Price = product.LocalPrice
            };
            _nutakuSystem.PostPayment(_lastInApp, PostPaymentCallback);
            if (_purchaseCompletionSource.Task.Status == UniTaskStatus.Pending) 
                _technicalMessageService.Open<ProcessPurchaseView>().Forget();
            return await _purchaseCompletionSource.Task;
        }
        
        private void PostPaymentCallback(bool success)
        {
            _purchaseCompletionSource.TrySetResult(success);
            OnPurchaseProcessed?.Invoke(_lastInApp.Id, success);
            _lastInApp = null;
            _technicalMessageService.Close<ProcessPurchaseView>();
        }

        public UniTask<bool> Confirm(string productId, Action purchaseUsing)
        {
            purchaseUsing?.Invoke();
            return UniTask.FromResult(true);
        }

        private ProductRow GetProductConfig(string productId)
        {
            if (!_sheet.Contains(productId)) return null;
            return _sheet[productId];
        }
    }
}