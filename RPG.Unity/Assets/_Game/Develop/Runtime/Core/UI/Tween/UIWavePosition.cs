using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Tween
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UIWavePosition : MonoBehaviour
    {
        [SerializeField, Required] private Transform _startPoint;
        [SerializeField, Required] private Transform _endPoint;
        [SerializeField, Required] private Transform _target;
        [SerializeField, MinValue(0)] private float _duration = 0.5f;
        [SerializeField] private Ease _ease = Ease.InOutSine;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _stopOnDisable = true;
        [SerializeField] private bool _useUnscaleTime;

        private Sequence _sequence;
        
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
            _target.transform.localPosition = _startPoint.localPosition;
            _sequence = Sequence.Create(-1, CycleMode.Restart, useUnscaledTime:_useUnscaleTime);
            _sequence.Chain(PrimeTween.Tween.LocalPosition(_target, _endPoint.localPosition, _duration / 2, _ease, useUnscaledTime: _useUnscaleTime));
            _sequence.Chain(PrimeTween.Tween.LocalPosition(_target, _startPoint.localPosition, _duration / 2, _ease, useUnscaledTime: _useUnscaleTime));
        }

        [Button]
        public void Stop()
        {
            _sequence.Stop();
        }
    }
}