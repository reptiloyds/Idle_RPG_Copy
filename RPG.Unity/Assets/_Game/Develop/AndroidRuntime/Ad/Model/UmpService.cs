using Cysharp.Threading.Tasks;
using GoogleMobileAds.Ump.Api;
using PleasantlyGames.RPG.Runtime.DebugUtilities;

namespace PleasantlyGames.RPG.AndroidRuntime.Ad.Model
{
    public sealed class UmpService
    {
        private UniTaskCompletionSource<bool> _completionSource;
        
        public async UniTask<bool> TryUpdateConsent(bool force = false)
        {
            if (force) 
                ConsentInformation.Reset();
            _completionSource = new UniTaskCompletionSource<bool>();

            var request = new ConsentRequestParameters()
            {
                ConsentDebugSettings = new ConsentDebugSettings()
                {
#if RPG_DEV
                    DebugGeography = DebugGeography.RegulatedUSState,
#endif
                    TestDeviceHashedIds = AdMobService.Const.TestDeviceIds,
                }
            };
            
            ConsentInformation.Update(request, OnConsentInfoUpdated);

            return await _completionSource.Task;
        }

        private void OnConsentInfoUpdated(FormError consentError)
        {
            if (consentError != null)
            {
                Logger.LogError(consentError.Message);
                Logger.LogError("UMP update failed.");
                _completionSource.TrySetResult(false);
                return;
            }

            if (ConsentInformation.CanRequestAds())
            {
                _completionSource.TrySetResult(true);
                Logger.Log("Already consented.");
                return;
            }
            
            ConsentForm.LoadAndShowConsentFormIfRequired(OnLoadedConsentForm);
        }

        private void OnLoadedConsentForm(FormError formError)
        {
            if (formError != null)
            {
                Logger.LogError(formError.Message);
                Logger.LogError("UMP update failed.");
                _completionSource.TrySetResult(false);
                return;
            }

            Logger.Log("Consent form filled");
            _completionSource.TrySetResult(true);
        }
    }
}