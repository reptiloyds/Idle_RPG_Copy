using System;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class TechnicalMessage : MonoBehaviour
    {
        [SerializeField] private RectTransform _animationTarget;

        [SerializeField] private bool _animateOpen = true;
        [HideIf("@this._animateOpen == false")]
        [SerializeField] private TweenSettings<Vector3> _openSettings = new()
        {
            startValue = new Vector3(0.3f, 0.3f, 0.3f),
            endValue = new Vector3(1f, 1f, 1f),
            settings = new TweenSettings(0.35f, Ease.OutBack, useUnscaledTime: true),
            startFromCurrent = false,
        };

        [SerializeField] private bool _animateClose = true;
        [HideIf("@this._animateClose == false")]
        [SerializeField] private TweenSettings<Vector3> _closeSettings = new()
        {
            startValue = new Vector3(1f, 1f, 1f),
            endValue = new Vector3(0.3f, 0.3f, 0.3f),
            settings = new TweenSettings(0.35f, Ease.InBack, useUnscaledTime: true),
            startFromCurrent = false,
        };
        
        private Tween _animation;

        public event Action<TechnicalMessage> OnClosed;

        public virtual void Open()
        {
            gameObject.SetActive(true);
            if (_animateOpen)
            {
                _animation.Stop();
                _animation = Tween.Scale(_animationTarget, _openSettings);   
            }
        }

        public virtual void Close()
        {
            if (_animateClose)
            {
                _animation.Stop();
                _animation = Tween.Scale(_animationTarget, _closeSettings).OnComplete(OnCompleteClose);   
            }
            else
                OnCompleteClose();
        }

        private void OnCompleteClose()
        {
            gameObject.SetActive(false);
            OnClosed?.Invoke(this);
        }
    }
}