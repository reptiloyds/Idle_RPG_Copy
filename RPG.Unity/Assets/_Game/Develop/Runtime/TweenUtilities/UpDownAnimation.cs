using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.TweenUtilities
{
    [DisallowMultipleComponent]
    public class UpDownAnimation : BaseAnimation
    {
        [SerializeField, Required] private Transform _target;
        [SerializeField] private TweenSettings<float> _tweenSettings;

        private Vector3 _originalPosition;
        private Tween _tween;
        
        private void Reset() => _target = transform;
        private void Awake() => _originalPosition = _target.localPosition;

        public override void Play()
        {
            _tween.Stop();
            
            _tween = Tween.LocalPositionY(_target, _originalPosition.y,
                _originalPosition.y + _tweenSettings.endValue,
                _tweenSettings.settings.duration,
                _tweenSettings.settings.ease,
                _tweenSettings.settings.cycles,
                _tweenSettings.settings.cycleMode);
        }

        public override void Stop()
        {
            _tween.Stop();
            _target.localPosition = _originalPosition;
        }
    }
}