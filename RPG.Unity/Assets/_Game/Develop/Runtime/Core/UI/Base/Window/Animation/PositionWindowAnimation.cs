using System;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Window.Animation
{
    public class PositionWindowAnimation : BaseWindowAnimation
    {
        [Flags]
        private enum WindowValueModification
        {
            Width = 1 << 0,
            Height = 1 << 1,
            Half = 1 << 2,
        }

        [SerializeField]
        private RectTransform _rootRectTransform;
        [SerializeField] private WindowValueModification _valueModification;
        [SerializeField] private List<RectTransform> _targets;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Open)")]
        private bool _openStartFromCurrentPosition;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Open)")]
        private TweenSettings<Vector2> _openSettings;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Close)")]
        private bool _closeStartFromCurrentPosition;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Close)")]
        private TweenSettings<Vector2> _closeSettings;

        private Vector2 _openStartPosition;
        private Vector2 _openEndPosition;

        private Vector2 _closeStartPosition;
        private Vector2 _closeEndPosition;

        private float Width => _rootRectTransform.rect.width;
        private float Height => _rootRectTransform.rect.height;

        private void Reset() => 
            ValidateRoot();

        private void OnValidate() => 
            ValidateRoot();

        private void Awake()
        {
            ValidateRoot();
            _openStartPosition = _openSettings.startValue;
            _openEndPosition = _openSettings.endValue;

            _closeStartPosition = _closeSettings.startValue;
            _closeEndPosition = _closeSettings.endValue;
        }

        private void ValidateRoot()
        {
            if(_rootRectTransform != null) return;
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null) 
                _rootRectTransform = canvas.GetComponent<RectTransform>();
        }

        public override float GetOpenDuration()
        {
            if (!_animationType.HasFlag(WindowAnimationType.Open)) return 0;
            
            return _openSettings.settings.duration;
        }

        public override float GetCloseDuration()
        {
            if (!_animationType.HasFlag(WindowAnimationType.Close)) return 0;
            
            return _closeSettings.settings.duration;
        }

        public override void Open()
        {
            if(!_animationType.HasFlag(WindowAnimationType.Open)) return;
            
            _openSettings = UpdateValues(_openSettings, _openStartPosition, _openEndPosition);
            
            foreach (var target in _targets) 
                PrimeTween.Tween.UIAnchoredPosition(target, _openSettings.WithDirection(true, _openStartFromCurrentPosition));
        }

        public override void Close()
        {
            if(!_animationType.HasFlag(WindowAnimationType.Close)) return;
            
            _closeSettings = UpdateValues(_closeSettings, _closeStartPosition, _closeEndPosition);
            
            foreach (var target in _targets) 
                PrimeTween.Tween.UIAnchoredPosition(target, _closeSettings.WithDirection(true, _closeStartFromCurrentPosition));
        }

        private TweenSettings<Vector2> UpdateValues(TweenSettings<Vector2> settings, Vector2 startPosition, Vector2 endPosition)
        {
            var divideK = _valueModification.HasFlag(WindowValueModification.Half) ? 2 : 1;
            
            if (_valueModification.HasFlag(WindowValueModification.Width))
            {
                var startValue = settings.startValue;
                if (startValue.x != 0) 
                    startValue.x = (Mathf.Sign(startPosition.x) * Width / divideK) + startPosition.x;
                
                var endValue = settings.endValue;
                if (endValue.x != 0) 
                    endValue.x = (Mathf.Sign(endPosition.x) * Width / divideK) + endPosition.x;

                settings.startValue = startValue;
                settings.endValue = endValue;
            }
            
            if (_valueModification.HasFlag(WindowValueModification.Height))
            {
                var startValue = settings.startValue;
                if (startValue.y != 0) 
                    startValue.y = (Mathf.Sign(startPosition.y) * Height / divideK) + startPosition.y;
                
                var endValue = settings.endValue;
                if (endValue.y != 0) 
                    endValue.y = (Mathf.Sign(endPosition.y) * Height / divideK) + endPosition.y;

                settings.startValue = startValue;
                settings.endValue = endValue;
            }

            return settings;
        }
    }
}