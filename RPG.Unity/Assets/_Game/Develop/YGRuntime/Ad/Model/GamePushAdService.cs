using GamePush;
using PleasantlyGames.RPG.Runtime.Ad.Model;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.Ad.Model
{
    public sealed class GamePushAdService : BaseAdService
    {
        [Preserve]
        public GamePushAdService() { }

        public override void Initialize()
        {
            base.Initialize();
            if (IsDisabled)
                return;
            
            if (GP_Ads.CanShowFullscreenBeforeGamePlay() && GP_Ads.IsPreloaderAvailable() && !GP_Ads.IsPreloaderPlaying())
                GP_Ads.ShowPreloader();

            if (!GP_Ads.IsStickyAvailable())
                return;
            
            if (!GP_Ads.IsStickyPlaying()) 
                GP_Ads.ShowSticky();
            
            GP_Ads.ShowFullscreen();
            
            GP_Ads.OnStickyClose += OnStickyClose;
            return;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (GP_Ads.IsStickyAvailable()) 
                GP_Ads.OnStickyClose -= OnStickyClose;
        }

        protected override bool IsRewardAdReady()
        {
#if UNITY_EDITOR
            return true;
#endif
            return GP_Ads.IsRewardedAvailable();
        }

        public override void ShowReward(string id)
        {
            base.ShowReward(id);

            if (IsDisabled)
            {
                RewardResult(true);
                return;
            } 
            
#if UNITY_EDITOR
            RewardResult(true);
            return;
#endif
            GP_Ads.ShowRewarded(onRewardedClose: RewardResult);
        }

        private static void OnStickyClose() => GP_Ads.ShowSticky();
    }
}
