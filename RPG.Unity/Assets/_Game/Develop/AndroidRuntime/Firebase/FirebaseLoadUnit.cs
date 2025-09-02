using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.AndroidRuntime.Firebase
{
    public class FirebaseLoadUnit : ILoadUnit
    {
        private readonly ThirdPartyEvents _thirdPartyEvents;
        private readonly PrivacyPolicyService _privacyPolicy;
        public string DescriptionToken => "FirebaseLoading";

        public FirebaseLoadUnit(ThirdPartyEvents thirdPartyEvents, PrivacyPolicyService privacyPolicy)
        {
            _thirdPartyEvents = thirdPartyEvents;
            _privacyPolicy = privacyPolicy;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            var status = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (status == DependencyStatus.Available)
            {
                Logger.Log("Firebase initialized!");
                var instance = FirebaseApp.DefaultInstance;
#if RPG_PROD
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(_privacyPolicy.IsAccepted);
#elif RPG_DEV
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(false);
#endif
            }
            else
            {
                Logger.LogError($"Firebase init failed: {status}");
                _thirdPartyEvents.InitializationFailedInvoke();
            }
            progress?.Report(1f);
        }
    }
}
