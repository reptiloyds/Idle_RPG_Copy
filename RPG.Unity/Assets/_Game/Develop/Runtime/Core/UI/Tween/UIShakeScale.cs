using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Tween
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UIShakeScale : MonoBehaviour
    {
        [SerializeField, Required] private Transform _target;
        [SerializeField] private ShakeSettings _shakeSettings;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _stopOnDisable = true;
        [SerializeField] private bool _resetScaleOnStop = true;

        private PrimeTween.Tween _tween;
        private Vector3 _originalScale;

        public float Duration => _shakeSettings.duration;

        private void Reset() => 
            _target = transform;

        private void OnEnable()
        {
            if (_playOnEnable) Play();
        }

        private void OnDisable()
        {
            if(_stopOnDisable) Stop();
        }

        [Button]
        public void Play()
        {
            Stop();
            _originalScale = _target.localScale;
            _tween = PrimeTween.Tween.PunchScale(_target, _shakeSettings);
        }

        [Button]
        public void Stop()
        {
            if (_resetScaleOnStop && _originalScale != Vector3.zero) 
                _target.localScale = _originalScale;
            _tween.Stop();
        }
    }
}