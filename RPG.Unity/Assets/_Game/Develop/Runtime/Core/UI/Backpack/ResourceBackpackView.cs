using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Backpack
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ResourceBackpackView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Transform _animationTarget;
        [SerializeField] private TweenSettings<Vector3> _openSettings;
        [SerializeField] private TweenSettings<Vector3> _closeSettings;
        
        private int _openRequest;
        private PrimeTween.Tween _tween;

        public RectTransform RectTransform => _rectTransform;
        
        private void Awake() => 
            _animationTarget.localScale = Vector3.zero;

        public void Open()
        {
            _openRequest++;
            if (_openRequest != 1) return;
            
            _tween.Stop();
            _tween = PrimeTween.Tween.Scale(_animationTarget, _openSettings);
        }

        public void Close()
        {
            _openRequest--;
            if(_openRequest != 0) return;

            _tween.Stop();
            _tween = PrimeTween.Tween.Scale(_animationTarget, _closeSettings);
        }
    }
}