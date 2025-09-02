using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.AndroidRuntime.Extensions;
using PleasantlyGames.RPG.AndroidRuntime.InApp.Model;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model;
using Unity.Services.Core;
using UnityEngine.Purchasing;
using VContainer;

namespace PleasantlyGames.RPG.AndroidRuntime.LoadUnits
{
    public class UnityPurchaseLoadUnit : ILoadUnit
    {
        private readonly BalanceContainer _balance;
        private readonly UnityPurchaseProvider _purchaseProvider;
        private readonly ThirdPartyEvents _thirdPartyEvents;

        private UniTaskCompletionSource _completionSource;
        
        public string DescriptionToken => "UnityPurchaseInitialization";

        public UnityPurchaseLoadUnit(BalanceContainer balance, UnityPurchaseProvider purchaseProvider,
            ThirdPartyEvents thirdPartyEvents)
        {
            _balance = balance;
            _purchaseProvider = purchaseProvider;
            _thirdPartyEvents = thirdPartyEvents;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            
            await UniTask.SwitchToMainThread();
            await UnityServices.InitializeAsync().AsUniTask();
            var productsSheet = _balance.Get<ProductsSheet>();
            
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (var config in productsSheet) 
                builder.AddProduct(config.Id, config.Type.ConvertToUnityType());

            _completionSource = new UniTaskCompletionSource();
            
            _purchaseProvider.OnInitialized += OnInitialized;
            _purchaseProvider.OnInitializedFailed += OnFailed;
            UnityPurchasing.Initialize(_purchaseProvider, builder);

            await _completionSource.Task;
            
            progress?.Report(1f);
        }

        private void OnInitialized() => 
            _completionSource.TrySetResult();

        private void OnFailed(InitializationFailureReason reason, string message)
        {
            Logger.LogError($"IAP init failed: {reason} {message}");
            _thirdPartyEvents.InitializationFailedInvoke();
            _completionSource.TrySetResult();
        }
    }
}