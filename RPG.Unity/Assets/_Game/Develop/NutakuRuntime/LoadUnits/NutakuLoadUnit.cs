using System;
using System.Threading;
using _Game.Scripts.Systems.Nutaku;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.NutakuRuntime.Analytics.Model;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model;
using R3;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.NutakuRuntime.LoadUnits
{
    public class NutakuLoadUnit : ILoadUnit
    {
        private readonly NutakuSystem _nutakuSystem;
        private readonly ThirdPartyEvents _thirdPartyEvents;
        private readonly NutakuAnalyticsService _analytics;
        private readonly CompositeDisposable _disposable = new();
        public string DescriptionToken => "NutakuLoading";
        private UniTaskCompletionSource _completionSource;

        [Preserve]
        public NutakuLoadUnit(NutakuSystem nutakuSystem, ThirdPartyEvents thirdPartyEvents, NutakuAnalyticsService analytics)
        {
            _analytics = analytics;
            _nutakuSystem = nutakuSystem;
            _thirdPartyEvents = thirdPartyEvents;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
//             var platformId = "2000003952";
// #if UNITY_WEBGL
//             platformId = "WebGLPlayer";
// #elif UNITY_ANDROID
//             platformId = "Android";
// #endif
            _completionSource = new UniTaskCompletionSource();
            _nutakuSystem.State
                .Skip(1)
                .Subscribe(HandleNutakuState)
                .AddTo(_disposable);
            _nutakuSystem.Init();
            await _completionSource.Task;
            progress?.Report(1);
        }

        private void HandleNutakuState(NutakuState nutakuState)
        {
            //var success = await _nutakuSystem.Initialize("firstdevuser", platformId);
            switch (nutakuState)
            {
                case NutakuState.NotInitialized:
                    Logger.Log("Nutaku not initialized");
                    break;
                case NutakuState.InitializationError:
                    Logger.Log("Nutaku initialization failed");
                    _thirdPartyEvents.InitializationFailedInvoke();
                    _analytics.SendNutakuInitializationStatus(false);
                    _completionSource.TrySetResult();
                    _disposable.Dispose();
                    break;
                case NutakuState.Initialized:
                    Logger.Log("Nutaku successfully initialized");
                    _analytics.SendNutakuInitializationStatus(true);
                    _completionSource.TrySetResult();
                    _disposable.Dispose();
                    break;
            }
        }
    }
}