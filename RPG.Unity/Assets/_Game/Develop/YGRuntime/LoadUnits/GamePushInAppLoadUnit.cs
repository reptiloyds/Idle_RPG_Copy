using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GamePush;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.YGRuntime.InAppPurchase.Model;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.LoadUnits
{
    public class GamePushInAppLoadUnit : ILoadUnit
    {
        [Inject] private GamePushInAppProvider _inAppProvider;
        
        private UniTaskCompletionSource _fetchProductTcs;
        private UniTaskCompletionSource _fetchPlayerPurchasesTcs;
        
        public string DescriptionToken => "GamePushInAppLoading";
        
        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            
            _fetchProductTcs = new UniTaskCompletionSource();
            _fetchPlayerPurchasesTcs = new UniTaskCompletionSource();
            GP_Payments.OnFetchProducts += OnProductsFetched;
            GP_Payments.OnFetchPlayerPurchases += OnPlayerPurchasesFetched;
            GP_Payments.Fetch();
            
            await UniTask.WhenAll(_fetchProductTcs.Task, _fetchPlayerPurchasesTcs.Task);
            
            progress?.Report(1);
        }
        
        private void OnProductsFetched(List<FetchProducts> products)
        {
            _inAppProvider.SetupProducts(products);
            GP_Payments.OnFetchProducts -= OnProductsFetched;
            _fetchProductTcs.TrySetResult();
        }

        private void OnPlayerPurchasesFetched(List<FetchPlayerPurchases> purchases)
        {
            _inAppProvider.SetupNonPurchased(purchases.Select(x => x.tag).ToList());
            GP_Payments.OnFetchPlayerPurchases -= OnPlayerPurchasesFetched;
            _fetchPlayerPurchasesTcs.TrySetResult();
        }
    }
}