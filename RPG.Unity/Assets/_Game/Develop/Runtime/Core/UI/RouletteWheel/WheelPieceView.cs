using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.RouletteWheel
{
    [DisallowMultipleComponent, HideMonoScript]
    public class WheelPieceView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private BaseButton _tooltipButton;
        [SerializeField] private RectTransform _tooltipTarget;

        [Inject] private TooltipFactory _tooltipFactory;
        
        private ResourceType _resourceType;

        public Sprite Sprite => _image.sprite;
        public string Text => _text.text;
        
        private void Awake()
        {
            _tooltipButton.OnClick += ShowTooltip;
        }

        private void OnDestroy()
        {
            _tooltipButton.OnClick -= ShowTooltip;
        }

        public void Setup(Sprite sprite, string text, ResourceType resourceType)
        {
            _resourceType = resourceType;
            _image.sprite = sprite;
            _text.SetText(text);
        }

        private void ShowTooltip()
        {
            _tooltipFactory.ShowResourceTooltip(_resourceType, _tooltipTarget);
        }
    }
}