using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Window.Animation
{
    public class ScaleWindowAnimation : BaseWindowAnimation
    {
        [SerializeField] private List<RectTransform> _targets;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Open)")]
        private bool _openStartFromCurrentScale;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Open)")]
        private TweenSettings<Vector3> _openSettings;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Close)")]
        private bool _closeStartFromCurrentScale;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Close)")]
        private TweenSettings<Vector3> _closeSettings;
        
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
                PrimeTween.Tween.Scale(target, _openSettings.WithDirection(true, _openStartFromCurrentScale)); 
        }

        public override void Close()
        {
            if(!_animationType.HasFlag(WindowAnimationType.Close)) return;
            
            foreach (var target in _targets) 
                PrimeTween.Tween.Scale(target, _closeSettings.WithDirection(true, _closeStartFromCurrentScale));
        }
    }
}