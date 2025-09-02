using System;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SubModeEnterResourceView : MonoBehaviour
    {
        [SerializeField, Required] private BaseButton _tooltipButton;
        [SerializeField, Required] private RectTransform _tooltipTarget;
        [SerializeField] private Image _enterResourceImage;
        [SerializeField] private TextMeshProUGUI _enterResourceAmount;
        [SerializeField] private Sprite _bonusResSprite;
        [SerializeField] private Color _defaultResourceColor;
        [SerializeField] private Color _zeroResourceColor;

        [Inject] private ResourceService _resourceService;
        [Inject] private IAdService _adService;
        [Inject] private TooltipFactory _tooltipFactory;

        private bool _isInitialized;
        private bool _isBonusEnter;
        private ResourceModel _enterResource;

        private void Start()
        {
            _tooltipButton.OnClick += ShowTooltip;
        }

        private void OnDestroy()
        {
            _tooltipButton.OnClick -= ShowTooltip;
        }

        private void ShowTooltip()
        {
            _tooltipFactory.ShowResourceTooltip(_enterResource.Type, _tooltipTarget);
        }

        private void Initialize()
        {
            _isInitialized = true;
            if (!_adService.IsDisabled.CurrentValue) 
                _adService.IsDisabled
                    .Subscribe(value => OnAdDisabled())
                    .AddTo(this);
        }

        private void OnAdDisabled()
        {
            if (_isBonusEnter) 
                SetBonusSprite();
        }

        public void Redraw(SubMode subMode)
        {
            if (!_isInitialized) 
                Initialize();
            
            _enterResource = subMode.EnterResource;
            
            if (subMode.IsEnterResourceEnough)
            {
                _isBonusEnter = false;
                _enterResourceImage.sprite = _enterResource.Sprite;
                _enterResourceAmount.color = _defaultResourceColor;
                RedrawEnterResourceAmount(_enterResource, subMode);
            }
            else
            {
                _isBonusEnter = true;
                if (subMode.BonusEnterAmount > 0)
                {
                    SetBonusSprite();
                    _enterResourceAmount.SetText($"{subMode.BonusEnterAmount}/{subMode.DailyEnterBonusAmount}");
                    _enterResourceAmount.color = _defaultResourceColor;
                }
                else
                {
                    _enterResourceImage.sprite = _enterResource.Sprite;
                    _enterResourceAmount.color = _zeroResourceColor;
                    RedrawEnterResourceAmount(_enterResource, subMode);
                }
            }
        }

        private void SetBonusSprite() => 
            _enterResourceImage.sprite = _bonusResSprite;

        private void RedrawEnterResourceAmount(ResourceModel resourceModel, SubMode subMode) => 
            _enterResourceAmount.SetText($"{resourceModel.Value}/{subMode.DailyEnterResourceAmount}");
    }
}