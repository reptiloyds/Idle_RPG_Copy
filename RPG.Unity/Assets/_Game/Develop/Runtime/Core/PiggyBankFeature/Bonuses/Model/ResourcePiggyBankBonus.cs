using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Data;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PrimeTween;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Model
{
    public class ResourcePiggyBankBonus : BasePiggyBankBonus
    {
        private ResourceService _resource;
        private ResourcePiggyBankRewardData _data;
        private Tween _resourceFxTween;

        public ResourceType ResourceType => _data.Type;

        public ResourcePiggyBankBonus(PiggyBankBonusData data,
            PiggyBankBonusesSheet.PiggyBankBonusRow config,
            ITranslator translator,
            ResourceService resource,
            ResourcePiggyBankRewardData resourceRewardData) : base(data, config, translator)
        {
            _data = resourceRewardData;
            _resource = resource;
        }

        public override void Collect(Vector3 vfxPosition, float vfxDelay)
        {
            base.Collect(vfxPosition, vfxDelay);
            _resource.AddResource(_data.Type, _data.Amount);
            
            _resourceFxTween.Stop();
            _resourceFxTween = Tween.Delay(vfxDelay, () => 
            { 
                _resource.SetFxRequest(ResourceFXRequest.Create(_data.Type, _data.Amount, spawnPosition: vfxPosition));
            });
        }

        public override Sprite GetIcon()
        {
            var resource = _resource.GetResource(_data.Type);
            return resource.Sprite;
        }

        public override int GetAmount()
        {
            return _data.Amount;
        }
    }
}