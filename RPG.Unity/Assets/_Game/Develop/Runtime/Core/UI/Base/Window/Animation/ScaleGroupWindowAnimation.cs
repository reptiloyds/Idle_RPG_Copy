using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Window.Animation
{
    public class ScaleGroupWindowAnimation : BaseWindowAnimation
    {
        [SerializeField] private bool _useUnscaledTime = true;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Open)")]
        private List<WindowTweens.ScaleGroup> _openGroups;
        [SerializeField, HideIf("@!this._animationType.HasFlag(WindowAnimationType.Close)")]
        private List<WindowTweens.ScaleGroup> _closeGroup;
        
        public override float GetOpenDuration()
        {
            if (!_animationType.HasFlag(WindowAnimationType.Open)) return 0;

            return GetMaxDuration(_openGroups);
        }

        public override float GetCloseDuration()
        {
            if (!_animationType.HasFlag(WindowAnimationType.Close)) return 0;
            
            return GetMaxDuration(_closeGroup);
        }

        public override void Open()
        {
            if(!_animationType.HasFlag(WindowAnimationType.Open)) return;
            
            OpenAnimation();
        }

        public override void Close()
        {
            if(!_animationType.HasFlag(WindowAnimationType.Close)) return;
            
            CloseAnimation();
        }

        private float GetMaxDuration(List<WindowTweens.ScaleGroup> scaleGroups)
        {
            float maxDuration = 0;
            foreach (var scaleGroup in scaleGroups)
            {
                var duration = scaleGroup.Delay + scaleGroup.Duration;
                if (duration > maxDuration)
                    maxDuration = duration;
            }

            return maxDuration;
        }

        private void OpenAnimation()
        {
            foreach (var scaleGroup in _openGroups)
            {
                foreach (var element in scaleGroup.Elements) 
                    element.localScale = Vector3.zero;

                if (scaleGroup.Delay <= 0)
                {
                    foreach (var element in scaleGroup.Elements) 
                        PrimeTween.Tween.Scale(element, Vector3.one, scaleGroup.Duration, scaleGroup.Ease, useUnscaledTime: _useUnscaledTime);
                }
                else
                {
                    PrimeTween.Tween.Delay(scaleGroup.Delay, () =>
                    {
                        foreach (var element in scaleGroup.Elements) 
                            PrimeTween.Tween.Scale(element, Vector3.one, scaleGroup.Duration, scaleGroup.Ease, useUnscaledTime: _useUnscaledTime);
                    }, useUnscaledTime: _useUnscaledTime);   
                }
            }
        }
        
        private void CloseAnimation()
        {
            foreach (var scaleGroup in _openGroups)
            {
                foreach (var element in scaleGroup.Elements) 
                    element.localScale = Vector3.one;
                
                PrimeTween.Tween.Delay(scaleGroup.Delay, () =>
                {
                    foreach (var element in scaleGroup.Elements) 
                        PrimeTween.Tween.Scale(element, Vector3.zero, scaleGroup.Duration, scaleGroup.Ease, useUnscaledTime: _useUnscaledTime);
                }, useUnscaledTime: _useUnscaledTime);
            }
        }
    }
}