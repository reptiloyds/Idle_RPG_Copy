using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.LoadUnits
{
    public class InternetConnectionLoadUnit : ILoadUnit
    {
        private readonly InternetConnectionService _connectionService;
        private IAnalyticsService _analytics;

        public string DescriptionToken => "ConnectionCheck";

        [Preserve]
        public InternetConnectionLoadUnit(InternetConnectionService connectionService, IAnalyticsService analytics)
        {
            _analytics = analytics;
            _connectionService = connectionService;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            await _connectionService.Initialize();
            progress?.Report(1f);
            _analytics.SendInternetConnectionChecked(_connectionService.HasConnection);
        }
    }
}