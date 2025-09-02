using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Roulette.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class RouletteRewardWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private BaseButton _continueButton;
        [SerializeField] private Image _rewardImage;
        [SerializeField] private TextMeshProUGUI _rewardAmountText;

        public RectTransform ImageRect => _rewardImage.rectTransform;

        public event Action<RouletteRewardWindow> OnContinue;

        protected override void Awake()
        {
            base.Awake();
            _continueButton.OnClick += OnContinueClick;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _continueButton.OnClick -= OnContinueClick;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        public void Setup(Sprite sprite, string text)
        {
            _rewardImage.sprite = sprite;
            _rewardAmountText.SetText(text);
        }

        private void OnContinueClick()
        {
            OnContinue?.Invoke(this);
            Close();
        }
    }
}