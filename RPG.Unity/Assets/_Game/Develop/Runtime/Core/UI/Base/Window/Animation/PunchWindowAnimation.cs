using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Window.Animation
{
    public class PunchWindowAnimation : BaseWindowAnimation
    {
        [SerializeField] private List<RectTransform> _targets;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Open)")]
        private ShakeSettings _openSettings;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Close)")]
        private ShakeSettings _closeSettings;
        
        public override float GetOpenDuration()
        {
            if (!_animationType.HasFlag(WindowAnimationType.Open)) return 0;
            
            return _openSettings.duration + _openSettings.startDelay;
        }

        public override float GetCloseDuration()
        {
            if (!_animationType.HasFlag(WindowAnimationType.Close)) return 0;
            
            return _closeSettings.duration + _closeSettings.startDelay;
        }

        public override void Open()
        {
            if(!_animationType.HasFlag(WindowAnimationType.Open)) return;
            
            foreach (var target in _targets) 
                PrimeTween.Tween.PunchScale(target, _openSettings); 
            
        }

        public override void Close()
        {
            if(!_animationType.HasFlag(WindowAnimationType.Close)) return;
            
            foreach (var target in _targets) 
                PrimeTween.Tween.PunchScale(target, _closeSettings);
        }
    }
}