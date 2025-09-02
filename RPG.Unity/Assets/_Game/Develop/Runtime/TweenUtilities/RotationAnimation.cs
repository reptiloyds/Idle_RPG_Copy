using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.TweenUtilities
{
    [DisallowMultipleComponent]
    public class RotationAnimation : BaseAnimation
    {
        [SerializeField, Required] private Transform _target;
        [SerializeField] private TweenSettings<float> _tweenSettings;
        
        [SerializeField] private bool _clockwise;

        private Quaternion _originalRotation;
        private Tween _tween;

        private void Reset() => _target = transform;
        private void Awake() => _originalRotation = _target.localRotation;

        public override void Play()
        {
            _tween.Stop();
            
            _tween = Tween.LocalEulerAngles(_target,
                Vector3.zero,
                new Vector3(0, _clockwise ? _tweenSettings.endValue : -_tweenSettings.endValue, 0),
                _tweenSettings.settings.duration,
                _tweenSettings.settings.ease,
                _tweenSettings.settings.cycles,
                _tweenSettings.settings.cycleMode);
        }

        public override void Stop()
        {
            _tween.Stop();
            _target.localRotation = _originalRotation;
        }
    }
}