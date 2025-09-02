using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Rewards.NoAds
{
    public class NoAdsProductReward : ProductReward
    {
        private readonly IAdService _adService;
        private readonly NoAdsProductRewardData _data;
        private readonly Sprite _sprite;
        private readonly string _name;
        
        public override ProductItemType Type => ProductItemType.NoAds;

        public override Sprite Sprite => _sprite;
        public override string Name => _name;

        public NoAdsProductReward(IAdService adService, NoAdsProductRewardData data,
            Sprite sprite, string name,
            Color backColor, ProductElem config) : base(backColor, config)
        {
            _adService = adService;
            _data = data;
            _sprite = sprite;
            _name = name;
        }

        public override void Apply() => 
            _adService.Disable();
    }
}