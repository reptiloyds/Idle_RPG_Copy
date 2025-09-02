using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits
{
    public class AnalyticsLoadUnit : ILoadUnit
    {
        private readonly IAnalyticsService _analytics;
        public string DescriptionToken => "AnalyticsLoading";

        [Preserve]
        public AnalyticsLoadUnit(IAnalyticsService analytics) => 
            _analytics = analytics;

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            await _analytics.Initialize();
            _analytics.SendAnalyticsSdkReady();
            _analytics.SendGameStart();
            progress?.Report(1);
        }
    }
}