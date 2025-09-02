using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Model;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.View
{
    public class PiggyBankBonusView : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _tooltipTarget;
        [SerializeField, Required] private BaseButton _collectButton;
        [SerializeField, FoldoutGroup("Visual")] private ButtonVisual[] _buttonVisuals;
        [SerializeField, Required] private Image _rewardIcon;
        [SerializeField, Required] private TMP_Text _rewardAmountText;
        [SerializeField, Required] private TMP_Text _needLevelText;
        [SerializeField, Required] private List<Image> _iconsForColoring;
        [SerializeField] private Color _collectedButtonColor;
        [SerializeField] private Transform _punchTarget;
        [SerializeField] private ShakeSettings _shakeSettings;

        [Inject] private TooltipFactory _tooltips;
        
        private BasePiggyBankBonus _model;
        private Tween _punchTween;
        public bool IsCollected => _model.IsCollected;
        public int NeedLevel => _model.LevelNeed;

        private void Start()
        {
            _collectButton.OnClick += ShowTooltip;
        }
        
        private void OnDestroy()
        {
            _punchTween.Stop();
            _collectButton.OnClick -= ShowTooltip;
        }

        public void Setup(BasePiggyBankBonus bonus)
        {
            _model = bonus;
            _rewardIcon.sprite = _model.GetIcon();
            _rewardAmountText.text = $"{_model.GetAmount()}x";
            _needLevelText.text = _model.GetNeedLevelText();
            UpdateView();
        }

        public void Punch()
        {
            if (_punchTween.isAlive)
                _punchTween.Complete();
            
            _punchTween = Tween.PunchScale(_punchTarget, _shakeSettings);
        }

        public void UpdateView()
        {
            foreach (var buttonVisual in _buttonVisuals)
                buttonVisual.SetInteractable(_model.IsCollected);

            if (_model.IsCollected)
            {
                foreach (var icon in _iconsForColoring) 
                    icon.color = _collectedButtonColor;
            }
        }

        private void ShowTooltip()
        {
            if (_model is ResourcePiggyBankBonus bonusModel)
                _tooltips.ShowResourceTooltip(bonusModel.ResourceType, _tooltipTarget);
        }

        public void Collect(Vector3 vfxPosition, float vfxDelay)
        {
            _model.Collect(vfxPosition, vfxDelay);
        }
    }
}