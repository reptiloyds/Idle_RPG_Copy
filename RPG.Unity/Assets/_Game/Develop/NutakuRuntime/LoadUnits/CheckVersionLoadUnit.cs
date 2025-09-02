using System;
using System.Threading;
using _Game.Scripts.Systems.Nutaku;
using _Game.Scripts.Systems.Server;
using _Game.Scripts.Systems.Server.Data;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.NutakuRuntime.Analytics.Model;
using PleasantlyGames.RPG.NutakuRuntime.TechnicalMessages;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.Model;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model;
using UnityEngine;
using UnityEngine.Scripting;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.NutakuRuntime.LoadUnits
{
    public class CheckVersionLoadUnit : ILoadUnit
    {
        private readonly NutakuSystem _nutakuSystem;
        private readonly ServerSystem _serverSystem;
        private readonly ThirdPartyEvents _thirdPartyEvents;
        private readonly TechnicalMessageService _technicalMessageService;
        private readonly NutakuAnalyticsService _analytics;
        private UniTaskCompletionSource _completionSource;

        private const bool _checkVersion = false;
        public string DescriptionToken => "CheckVersion";

        [Preserve]
        public CheckVersionLoadUnit(NutakuSystem nutakuSystem, ServerSystem serverSystem,
            ThirdPartyEvents thirdPartyEvents, TechnicalMessageService technicalMessageService,
            NutakuAnalyticsService analytics)
        {
            _analytics = analytics;
            _nutakuSystem = nutakuSystem;
            _serverSystem = serverSystem;
            _thirdPartyEvents = thirdPartyEvents;
            _technicalMessageService = technicalMessageService;
        }
        
        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            if(!_serverSystem.IsActive) return;
            if(!_checkVersion) return;
            _completionSource = new UniTaskCompletionSource();
            ServerApi.SendVersionCheckRequest(Application.version, _nutakuSystem.PlatformId, SendVersionCallback);
            progress?.Report(1f);
            await _completionSource.Task;
            Logger.Log("Version checked");
        }

        private void SendVersionCallback(ServerRequestResultData resultData)
        {
            if(resultData.ResultType != ServerRequestResult.Success)
            {
                _technicalMessageService.Open<GameUpdateRequiredView>().Forget();
                _thirdPartyEvents.InitializationFailedInvoke(false);
                _analytics.SendNutakuVersionCheckStatus(false);
            }

            _analytics.SendNutakuVersionCheckStatus(true);
            _analytics.SendNutakuVersionAndPlatformId(Application.version, _nutakuSystem.PlatformId);
            _completionSource.TrySetResult();
        }
    }
}