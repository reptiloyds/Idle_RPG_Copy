using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Tween
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UIPunchRotation : MonoBehaviour
    {
        [SerializeField, Required] private Transform _target;
        [SerializeField, Required] private ShakeSettings _shakeSettings;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _stopOnDisable = true;
        [SerializeField] private bool _resetRotationOnStop = true;
        
        private PrimeTween.Tween _tween;
        private Quaternion _originalRotation;
        
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
            _originalRotation = _target.localRotation;
            _tween = PrimeTween.Tween.PunchLocalRotation(_target, _shakeSettings);
        }

        [Button]
        public void Stop()
        {
            if (_resetRotationOnStop) 
                _target.localRotation = _originalRotation;
            _tween.Stop();
        }
    }
}