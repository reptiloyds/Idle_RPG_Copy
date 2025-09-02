using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using PleasantlyGames.RPG.AndroidRuntime.Ad.Model;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model;

namespace PleasantlyGames.RPG.AndroidRuntime.LoadUnits
{
    public class AdMobLoadUnit : ILoadUnit
    {
        private readonly ThirdPartyEvents _thirdPartyEvents;
        private readonly UmpService _umpService;
        private UniTaskCompletionSource _initCompletionSource;

        public string DescriptionToken => "AdMobLoading";

        public AdMobLoadUnit(ThirdPartyEvents thirdPartyEvents, UmpService umpService)
        {
            _thirdPartyEvents = thirdPartyEvents;
            _umpService = umpService;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            _initCompletionSource = new UniTaskCompletionSource();
            
#if UNITY_EDITOR
            InitializeAdMob();
#else
            if (!await _umpService.TryUpdateConsent())
                _thirdPartyEvents.InitializationFailedInvoke();
            else
            {
                if(ConsentInformation.CanRequestAds())
                    InitializeAdMob();
                else
                    _initCompletionSource.TrySetResult();
            }
#endif

            await _initCompletionSource.Task;
            progress?.Report(1f);
        }

        private void InitializeAdMob()
        {
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            RequestConfiguration config = new RequestConfiguration
            {
                TestDeviceIds = AdMobService.Const.TestDeviceIds,
                TagForChildDirectedTreatment = TagForChildDirectedTreatment.Unspecified,
                TagForUnderAgeOfConsent = TagForUnderAgeOfConsent.Unspecified
            };
            MobileAds.SetRequestConfiguration(config);
            MobileAds.Initialize(OnInitialized);
        }

        private void OnInitialized(InitializationStatus initStatus)
        {
            if (initStatus == null)
            {
                Logger.LogError("Google Mobile Ads initialization failed.");
                _thirdPartyEvents.InitializationFailedInvoke();
            }
            else
            {
                Logger.Log("Google Mobile Ads initialization complete.");
                _initCompletionSource.TrySetResult();
            }
        }
    }
}