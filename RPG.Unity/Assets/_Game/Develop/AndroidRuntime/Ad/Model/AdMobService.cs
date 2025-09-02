using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using PleasantlyGames.RPG.Runtime.Ad.Model;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.Model;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.AndroidRuntime.Ad.Model
{
    public class AdMobService : BaseAdService
    {
        #region IDs

        public static class Const
        {
            private const string TEST_REWARD_ID = "ca-app-pub-3940256099942544/5224354917";
            private const string PROD_REWARD_ID = "ca-app-pub-3200345818645197/2401759446";

            public const int REFRESH_DELAY_MIN = 20;

            public static readonly List<string> TestDeviceIds = new()
            {
                "8AD46EBEC4194D285EC00252A3242A22"
            };

            public static string GetRewardId()
            {
#if RPG_PROD
                return PROD_REWARD_ID;
#endif
                return TEST_REWARD_ID;
            }
        }

        #endregion

        
        [Inject] private PrivacyPolicyService _privacyPolicy;

        private Dictionary<string, string> _extras;
        
        private RewardedAd _rewardedAd;
        private bool _isRewardAdLoading;
        private bool _rewardAdCompleted;
        private IDisposable _autoReloadSubscription;

        public override void Initialize()
        {
            base.Initialize();
#if UNITY_EDITOR
            CompleteInitialization();
#else
            if (ConsentInformation.CanRequestAds()) CompleteInitialization();
#endif
        }

        private void CompleteInitialization()
        {
            _extras = new Dictionary<string, string> { { "npa", _privacyPolicy.IsAccepted ? "0" : "1" } };
            LoadRewardAd();
            StartAutoReload();
        }

        private void StartAutoReload()
        {
            _autoReloadSubscription = Observable
                .Interval(TimeSpan.FromMinutes(Const.REFRESH_DELAY_MIN))
                .Subscribe(_ => ValidateRewards());
        }

        private void ValidateRewards()
        {
            if (_rewardedAd == null || !_rewardedAd.CanShowAd())
                LoadRewardAd();
        }

        private void LoadRewardAd()
        {
            if (_isRewardAdLoading) return;
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            _isRewardAdLoading = true;
            var adRequest = new AdRequest()
            {
                Extras = _extras,
            };
            RewardedAd.Load(Const.GetRewardId(), adRequest, LoadRewardCallback);
        }

        private void LoadRewardCallback(RewardedAd ad, LoadAdError error)
        {
            _isRewardAdLoading = false;
            if (error != null || ad == null)
                return;
            _rewardedAd = ad;
            RefreshReward();
        }

        protected override bool IsRewardAdReady() => 
            _rewardedAd != null && _rewardedAd.CanShowAd();

        public override void ShowReward(string id)
        {
            if (!CanShowReward()) return;
            base.ShowReward(id);
            
            if (IsDisabled)
            {
                RewardResult(true);
                return;
            } 

            _rewardAdCompleted = false;
            _rewardedAd.OnAdFullScreenContentClosed += OnAdRewardClosed;
            _rewardedAd.OnAdFullScreenContentFailed += OnAdRewardFailed;
            _rewardedAd.Show(_ => _rewardAdCompleted = true);
        }

        private async void OnAdRewardClosed()
        {
            await UniTask.SwitchToMainThread();
            _rewardedAd.OnAdFullScreenContentClosed -= OnAdRewardClosed;
            _rewardedAd.OnAdFullScreenContentFailed -= OnAdRewardFailed;
            RewardResult(_rewardAdCompleted);
            LoadRewardAd();
        }

        private async void OnAdRewardFailed(AdError adError)
        {
            await UniTask.SwitchToMainThread();
            _rewardedAd.OnAdFullScreenContentClosed -= OnAdRewardClosed;
            _rewardedAd.OnAdFullScreenContentFailed -= OnAdRewardFailed;
            RewardResult(false);
            LoadRewardAd();
        }

        public override void Dispose()
        {
            base.Dispose();

            _autoReloadSubscription?.Dispose();
        }
    }
}