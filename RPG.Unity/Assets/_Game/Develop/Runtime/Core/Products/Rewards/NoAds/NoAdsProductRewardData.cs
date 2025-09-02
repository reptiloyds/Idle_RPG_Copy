using System;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Rewards.NoAds
{
    [Serializable]
    public class NoAdsProductRewardData
    {
        public string ImageName;
        public string NameToken;
        
        [Preserve]
        public NoAdsProductRewardData()
        {
            
        }
    }
}