using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Balance.Contract;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits
{
    public class BalanceLoadUnit : ILoadUnit
    {
        private readonly IBalanceProvider _balanceProvider;
        private readonly ThirdPartyEvents _thirdPartyEvents;
        private IAnalyticsService _analytics;

        public string DescriptionToken => "BalanceLoading";

        [Preserve]
        public BalanceLoadUnit(IBalanceProvider balanceProvider, ThirdPartyEvents thirdPartyEvents, IAnalyticsService analytics)
        {
            _analytics = analytics;
            _balanceProvider = balanceProvider;
            _thirdPartyEvents = thirdPartyEvents;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            var success = await _balanceProvider.LoadAsync();
            if (success)
            {
                progress?.Report(1);
                Logger.Log("Balance loaded");
                _analytics.SendBalanceLoadingStatus(true);
            }
            else
            {
                _thirdPartyEvents.InitializationFailedInvoke();
                Logger.LogError("Balance load failed");
                _analytics.SendBalanceLoadingStatus(false);
            }
        }
    }
}