using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Resource
{
    public class ResourceProductReward : ProductReward
    {
        private readonly ResourceService _resourceService;
        private readonly ResourceProductRewardData _data;

        private Sprite _sprite;
        private readonly string _name;
        
        public override ProductItemType Type => ProductItemType.Resource;
        public override Sprite Sprite => _sprite;
        public override string Name => _name;

        public ResourceProductReward(ResourceService resourceService,
            ResourceProductRewardData data, Color backColor, ProductElem config) : base(backColor, config)
        {
            _resourceService = resourceService;
            _data = data;
            _name = _data.Amount.ToString();
        }

        public override void Initialize()
        {
            base.Initialize();

            _sprite = _resourceService.GetResource(_data.Type).Sprite;
        }

        public override void Apply() => 
            _resourceService.AddResource(_data.Type, _data.Amount);

        public override object GetCertainProductType()
        {
            return _data.Type;
        }
    }
}