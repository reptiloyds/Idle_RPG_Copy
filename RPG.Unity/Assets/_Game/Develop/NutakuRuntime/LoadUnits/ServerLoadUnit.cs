using System;
using System.Threading;
using _Game.Scripts.Systems.Nutaku;
using _Game.Scripts.Systems.Server;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.NutakuRuntime.Analytics.Model;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.NutakuRuntime.LoadUnits
{
    public class ServerLoadUnit : ILoadUnit
    {
        private readonly ServerSystem _serverSystem;
        private readonly ThirdPartyEvents _thirdPartyEvents;
        private readonly NutakuSystem _nutakuSystem;
        private readonly NutakuAnalyticsService _analytics;
        private readonly UniTaskCompletionSource _completionSource = new();
        public string DescriptionToken => "ServerLoading";

        [Preserve]
        public ServerLoadUnit(ServerSystem serverSystem, NutakuSystem nutakuSystem, ThirdPartyEvents thirdPartyEvents, NutakuAnalyticsService analytics)
        {
            _analytics = analytics;
            _serverSystem = serverSystem;
            _nutakuSystem = nutakuSystem;
            _thirdPartyEvents = thirdPartyEvents;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            _serverSystem.OnAuthSuccess += OnAuthSuccess;
            _serverSystem.SetAuthData(_nutakuSystem.AccessToken, _nutakuSystem.NumericId);
            await _completionSource.Task;
            // var success = await _serverSystem.Auth();
            // if (!success)
            // {
            //     _thirdPartyEvents.InitializationFailedInvoke();
            //     _analytics.SendNutakuConnectionStatus(false);
            // }
            // else
            // {
            //     progress?.Report(1f);
            //     _analytics.SendNutakuConnectionStatus(true);
            // }
        }

        private void OnAuthSuccess()
        {
            Logger.Log("Server loaded");
            _analytics.SendNutakuConnectionStatus(true);
            _completionSource.TrySetResult();
        }
    }
}