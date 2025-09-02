
using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.UI.Mediator;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.UI
{
    public class GameModeResultWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField, BoxGroup("Animation")] private Graphic[] _fadeAnimationTargets;
        [SerializeField, BoxGroup("Animation")] private float _fadeDuration = 1;
        [SerializeField, BoxGroup("Animation")] private float _fadeOpenDelay = 0.35f;
        
        [SerializeField] private GameObject _victoryGameObject;
        [SerializeField] private GameObject _defeatGameObject;

        [SerializeField] private Image _background;

        [Inject] private UIGameModeResultMediator _mediator;
        
        protected override void OpenAnimation(Action callback)
        {
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime);
            WindowTweens.OpenFadeTween(_fadeAnimationTargets, UseUnscaledTime, callback, _fadeDuration, _fadeOpenDelay);
        }

        protected override void CloseAnimation(Action callback)
        {
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, endValueK: 0);
            WindowTweens.CloseFadeTween(_fadeAnimationTargets, UseUnscaledTime, callback, _fadeDuration);
        }

        public override void Open()
        {
            base.Open();
            var color = _background.color;
            color = new Color(color.r, color.g, color.b, 0);
            _background.color = color;
        }

        protected override void CompleteOpening()
        {
            base.CompleteOpening();
            _mediator.OnResultWasPresented();
            Close();
        }

        protected override void CompleteClosing()
        {
            base.CompleteClosing();
            _mediator.OnResultWasClosed();
        }

        public void SetVictoryVisual()
        {
            _victoryGameObject.SetActive(true);
            _defeatGameObject.SetActive(false);
        }

        public void SetDefeatVisual()
        {
            _victoryGameObject.SetActive(false);
            _defeatGameObject.SetActive(true);
        }
    }
}