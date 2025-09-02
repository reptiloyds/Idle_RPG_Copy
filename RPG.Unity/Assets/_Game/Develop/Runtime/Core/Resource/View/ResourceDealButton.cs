using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Resource.Defenition;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.View
{
    public class ResourceDealButton : BaseDealButton
    {
        [SerializeField] private List<Image> _resourceImages;
        [SerializeField] private TextMeshProUGUI _priceText;

        [Inject] private ResourceConfiguration _configuration;
        [Inject] private ResourceService _resourceService;
        
        private ResourceModel _resourceModel;
        private BigDouble.Runtime.BigDouble _price;
        
        private const string FREE_TOKEN = "free";

        public ResourceType Type => _resourceModel.Type;
        public BigDouble.Runtime.BigDouble Value => _price;

        private void OnEnable()
        {
            if(_resourceModel != null)
                AutoUpdateInteraction();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_resourceModel != null)
                _resourceModel.OnChange -= ResourceModelChange;
        }

        private void ResourceModelChange() =>
            AutoUpdateInteraction();

        public void Redraw(ResourceType type, BigDouble.Runtime.BigDouble value)
        {
            if (_resourceModel == null || _resourceModel.Type != type)
            {
                if(_resourceModel != null)
                    _resourceModel.OnChange -= ResourceModelChange;
                _resourceModel = _resourceService.GetResource(type);
                _resourceModel.OnChange += ResourceModelChange;
                
                var sprite = _resourceModel.Sprite;
                foreach (var resourceImage in _resourceImages)
                    resourceImage.sprite = sprite;
            }
            
            _price = value;
            _priceText.SetText(value == 0 ? FREE_TOKEN : StringExtension.Instance.CutBigDouble(value));
            
            AutoUpdateInteraction();
        }

        protected override void Click()
        {
            base.Click();
            
            AutoUpdateInteraction();
            
            var isEnough = _resourceModel.IsEnough(_price);
            
            if (!isEnough)
            {
                if (!_configuration.ShouldOfferInApp(_resourceModel.Type)) return;
                //TODO: Offer in app
                return;
            }
            
            _resourceModel.Spend(_price);
            CompleteDeal();
        }

        protected override void AutoUpdateInteraction()
        {
            base.AutoUpdateInteraction();
            
            if(!IsAutoUpdatable) return;
            if(!gameObject.activeSelf) return;
            var result = _resourceModel.IsEnough(_price);
            SetInteractable(result || _configuration.ShouldOfferInApp(_resourceModel.Type));
        }
    }
}