using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Tween
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UIMovePosition : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _target;
        [SerializeField] private TweenSettings<Vector2> _settings;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _stopOnDisable = true;
        [SerializeField] private bool _resetPositionOnStop = true;

        private PrimeTween.Tween _tween;
        private Vector2 _originalPosition;

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
            _originalPosition = _target.anchoredPosition;
            _tween = PrimeTween.Tween.UIAnchoredPosition(_target, _settings);
        }

        [Button]
        public void Stop()
        {
            if (_resetPositionOnStop) 
                _target.anchoredPosition = _originalPosition;
            _tween.Stop();
        }
    }
}
