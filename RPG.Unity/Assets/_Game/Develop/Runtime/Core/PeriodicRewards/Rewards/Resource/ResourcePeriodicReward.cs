using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Resource
{
    public class ResourcePeriodicReward : PeriodicReward
    {
        private readonly ResourceService _resourceService;
        private readonly ResourcePeriodicData _data;
        private ResourceModel _resource;
        
        public ResourcePeriodicReward(PeriodicRewardSheet.Elem config, ISpriteProvider spriteProvider, ITranslator translator,
            ResourceService resourceService) : base(config, spriteProvider, translator)
        {
            _resourceService = resourceService;
            _data = JsonConvert.DeserializeObject<ResourcePeriodicData>(config.RewardJSON);
        }

        public override void Initialize()
        {
            _resource = _resourceService.GetResource(_data.ResourceType);
            base.Initialize();
            Text = $"X{_data.Amount}";
            TypeText = Translator.Translate(_resource.Type.ToString());
        }

        public override void Apply(Vector3 viewWorldPosition) => 
            _resourceService.AddResource(_data.ResourceType, _data.Amount, ResourceFXRequest.Create(spawnPosition: viewWorldPosition));

        protected override Sprite GetImageInternal() => 
            _resource.Sprite;
    }
}