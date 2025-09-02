using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Tween
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UIScale : MonoBehaviour
    {
        [SerializeField, Required] private Transform _target;
        [SerializeField] private TweenSettings<Vector3> _settings;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _stopOnDisable = true;
        [SerializeField] private bool _resetScaleOnStop = true;

        private PrimeTween.Tween _tween;
        private Vector3 _originalScale;
        private bool _isPlaying;

        public Transform Target => _target;

        public float Duration =>
            _settings.settings.startDelay + _settings.settings.duration + _settings.settings.endDelay;

        private void Reset() => 
            _target = transform;

        private void OnEnable()
        {
            if (_playOnEnable)
                Play();
        }

        private void OnDisable()
        {
            if(_stopOnDisable)
                Stop();
        }

        public void SetSettings(TweenSettings<Vector3> settings)
        {
            _settings = settings;
            if(_isPlaying)
                Play();
        }

        [Button]
        public void Play()
        {
            Stop();
            _originalScale = _target.localScale;
            _tween = PrimeTween.Tween.Scale(_target, _settings);
            _isPlaying = true;
        }

        [Button]
        public void Stop()
        {
            if(!_isPlaying) return;
            if (_resetScaleOnStop && _originalScale != Vector3.zero) 
                _target.localScale = _originalScale;
            _tween.Stop();
            _isPlaying = false;
        }
    }
}