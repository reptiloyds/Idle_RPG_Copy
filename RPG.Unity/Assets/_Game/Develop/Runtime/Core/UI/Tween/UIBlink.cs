using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Tween
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UIBlink : MonoBehaviour
    {
        [SerializeField, Required] private Transform _startPoint;
        [SerializeField, Required] private Transform _endPoint;
        [SerializeField, Required] private Transform _target;
        [SerializeField, MinValue(0)] private float _duration = 0.5f;
        [SerializeField] private Ease _ease = Ease.InOutSine;
        [SerializeField] private int _cycle = 1;
        [SerializeField] private CycleMode _cycleMode = CycleMode.Restart;
        [SerializeField] private float _startDelay;
        [SerializeField] private float _endDelay;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _stopOnDisable = true;

        private PrimeTween.Tween _tween;
        
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
            _target.gameObject.SetActive(true);
            _target.transform.localPosition = _startPoint.localPosition;
            _tween = PrimeTween.Tween.LocalPosition(_target, _endPoint.localPosition, _duration, _ease, _cycle, _cycleMode, _startDelay, _endDelay);
        }

        [Button]
        public void Stop()
        {
            _target.gameObject.SetActive(false);
            _tween.Stop();
        }
    }
}