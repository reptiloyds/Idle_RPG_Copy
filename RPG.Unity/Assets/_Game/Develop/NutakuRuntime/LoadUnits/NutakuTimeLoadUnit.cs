using System;
using System.Threading;
using _Game.Scripts.Systems.Server;
using _Game.Scripts.Systems.Server.Data;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.NutakuRuntime.Analytics.Model;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using VContainer;

namespace PleasantlyGames.RPG.NutakuRuntime.LoadUnits
{
    public class NutakuTimeLoadUnit : ILoadUnit
    {
        [Inject] private TimeService _timeService;
        private UniTaskCompletionSource _completionSource;
        private NutakuAnalyticsService _analytics;

        public string DescriptionToken => "TimeLoading";

        [Preserve]
        public NutakuTimeLoadUnit(NutakuAnalyticsService analytics)
        {
            _analytics = analytics;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            _completionSource = new UniTaskCompletionSource();
            ServerApi.GetServerTime(ServerTimeCallback);
            await _completionSource.Task;
            
            progress?.Report(1f);
            
            Logger.Log("Time fetched");
        }

        private void ServerTimeCallback(ServerRequestResultData requestResultData)
        {
            if (requestResultData.ResultType == ServerRequestResult.Success)
            {
                _timeService.SetupServerStartTime(GetDateTime(requestResultData.Result));
                _analytics.SendNutakuTimeFetchedStatus(true);
            }
            else
            {
                Logger.LogWarning("Failed to fetch time" );
                SetupLocalTime();
                _analytics.SendNutakuTimeFetchedStatus(false);
            }

            _completionSource.TrySetResult();
        }

        private DateTime GetDateTime(string input)
        {
#if UNITY_EDITOR
            return DateTime.Now;
#endif
            input = input.Trim('"');
            var result = DateTime.Parse(input);
            return result;
        }

        private void SetupLocalTime() => 
            _timeService.SetupServerStartTime(DateTime.UtcNow);
    }
}