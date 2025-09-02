using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Window.Animation
{
    public class FadeWindowAnimation : BaseWindowAnimation
    {
        [SerializeField] private List<CanvasGroup> _targets;
        [SerializeField] private List<Image> _imageTargets;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Open)")]
        private bool _openStartFromCurrentAlpha;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Open)")]
        private TweenSettings<float> _openSettings;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Close)")]
        private bool _closeStartFromCurrentAlpha;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Close)")]
        private TweenSettings<float> _closeSettings;
        
        public override float GetOpenDuration()
        {
            if (!_animationType.HasFlag(WindowAnimationType.Open)) return 0;
            
            return _openSettings.settings.duration + _openSettings.settings.startDelay;
        }

        public override float GetCloseDuration()
        {
            if (!_animationType.HasFlag(WindowAnimationType.Close)) return 0;
            
            return _closeSettings.settings.duration + _closeSettings.settings.startDelay;
        }

        public override void Open()
        {
            if(!_animationType.HasFlag(WindowAnimationType.Open)) return;
            
            foreach (var target in _targets) 
                PrimeTween.Tween.Alpha(target, _openSettings.WithDirection(true, _openStartFromCurrentAlpha));
            foreach (var image in _imageTargets) 
                PrimeTween.Tween.Alpha(image, _openSettings.WithDirection(true, _openStartFromCurrentAlpha));
        }

        public override void Close()
        {
            if(!_animationType.HasFlag(WindowAnimationType.Close)) return;
            
            foreach (var target in _targets) 
                PrimeTween.Tween.Alpha(target, _closeSettings.WithDirection(true, _closeStartFromCurrentAlpha));
            foreach (var image in _imageTargets) 
                PrimeTween.Tween.Alpha(image, _closeSettings.WithDirection(true, _closeStartFromCurrentAlpha));
        }
    }
}