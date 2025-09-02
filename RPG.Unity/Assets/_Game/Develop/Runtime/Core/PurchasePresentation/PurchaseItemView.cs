using System.Collections.Generic;
using AssetKits.ParticleImage;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Tween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PurchasePresentation
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PurchaseItemView : MonoBehaviour
    {
        [SerializeField] private BaseButton _tooltipsButton;
        [SerializeField] private RectTransform _tooltipTarget;
        [SerializeField] private Image _image;
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _infoText;
        [SerializeField] private List<ParticleImage> _presentParticle;
        [SerializeField] private List<UIScale> _scaleAnimations;

        [Inject] private TooltipFactory _tooltipFactory;
        private object _certainProductType;

        private void Start()
        {
            _tooltipsButton.OnClick += ShowTooltip;
        }

        private void OnDestroy()
        {
            _tooltipsButton.OnClick -= ShowTooltip;
        }

        private void ShowTooltip()
        {
            if (_certainProductType == null)
                return;
            
            switch (_certainProductType)
            {
                case ResourceType resource:
                    _tooltipFactory.ShowResourceTooltip(resource, _tooltipTarget);
                    break;
                case ItemType item:
                    _tooltipFactory.ShowItemTooltip(item);
                    break;
            }
        }

        public void Setup(Sprite sprite, Color color, string infoText, object certainProductType)
        {
            _certainProductType = certainProductType;
            _image.sprite = sprite;
            _background.color = color;
            _infoText.gameObject.SetActive(!string.IsNullOrEmpty(infoText));
            if(_infoText.gameObject.activeSelf)
                _infoText.SetText(infoText);
        }

        public void PlayPresentAnimation()
        {
            foreach (var particle in _presentParticle) 
                particle.Play();

            foreach (var scaleAnimation in _scaleAnimations) 
                scaleAnimation.Play();
        }

        public void Enable() => 
            gameObject.SetActive(true);

        public void Disable() => 
            gameObject.SetActive(false);
    }
}