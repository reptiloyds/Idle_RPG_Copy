using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Window
{
    public static class WindowTweens
    {
        private static readonly TweenSettings<Vector3> OpenScaleSettings = new()
        {
            startValue = 0.3f * Vector3.one,
            endValue = Vector3.one,
            settings = new TweenSettings()
            {
                ease = Ease.OutBack,
                duration = 0.35f,
            }
        };
        
        public static void OpenScaleTween(ICollection<RectTransform> targets, bool unscaledTime, Action callback = null, float startValueK = 0.3f, float endValueK = 1f)
        {
            var sequence = Sequence.Create(useUnscaledTime: unscaledTime);
            var openSettings = OpenScaleSettings;
            openSettings.startValue = Vector3.one * startValueK;
            openSettings.endValue = Vector3.one * endValueK;
            openSettings.settings.useUnscaledTime = unscaledTime;
            foreach (var target in targets)
                sequence.Group(PrimeTween.Tween.Scale(target, openSettings.WithDirection(true, false)));
            if (callback != null) sequence.OnComplete(callback);
        }
        
        private static readonly TweenSettings<Vector3> CloseScaleSettings = new()
        {
            settings = new TweenSettings()
            {
                ease = Ease.InBack,
                duration = 0.35f,
            }
        };

        public static void CloseScaleTween(ICollection<RectTransform> targets, bool unscaledTime, Action callback = null, float startValueK = 1, float endValueK = 0.3f)
        {
            var sequence = Sequence.Create(useUnscaledTime: unscaledTime);
            var closeSettings = CloseScaleSettings;
            closeSettings.startValue = Vector3.one * startValueK;
            closeSettings.endValue = Vector3.one * endValueK;
            closeSettings.settings.useUnscaledTime = unscaledTime;
            foreach (var target in targets)
                sequence.Group(PrimeTween.Tween.Scale(target, closeSettings.WithDirection(true, false)));
            if (callback != null) sequence.OnComplete(callback);
        }
        
        public enum VerticalAnimationType
        {
            FromTop = 0,
            FromBottom = 1,
            FromLeft = 2,
            FromRight = 3,
        }
        
        private static readonly TweenSettings<Vector2> OpenPositionSettings = new()
        {
            startValue = Vector2.one,
            endValue = Vector2.one,
            settings = new TweenSettings()
            {
                ease = Ease.OutSine,
                duration = 0.35f,
            }
        };

        public static void OpenPositionTween(RectTransform target, bool unscaledTime, Action callback = null, VerticalAnimationType type = VerticalAnimationType.FromTop)
        {
            var startPosition = target.anchoredPosition;
            switch (type)
            {
                case VerticalAnimationType.FromTop:
                    startPosition.y += target.rect.height;
                    break;
                case VerticalAnimationType.FromBottom:
                    startPosition.y -= target.rect.height;
                    break;
                case VerticalAnimationType.FromLeft:
                    startPosition.x -= target.rect.width;
                    break;
                case VerticalAnimationType.FromRight:
                    startPosition.x += target.rect.width;
                    break;
            }

            var settings = OpenPositionSettings;
            settings.startValue = startPosition;
            settings.endValue = Vector3.zero;
            settings.settings.useUnscaledTime = unscaledTime;
            var tween = PrimeTween.Tween.UIAnchoredPosition(target, settings.WithDirection(true, false));
            if (callback != null) tween.OnComplete(callback);
        }

        private static readonly TweenSettings<float> OpenFadeSettings = new()
        {
            startValue = 0f,
            endValue = 1f,
            settings = new TweenSettings()
            {
                ease = Ease.Default,
                duration = 0.35f,
            }
        };

        public static void OpenFadeTween(ICollection<Graphic> targets, bool unscaledTime, Action callback = null, float duration = 0.35f, float delay = 0)
        {
            var settings = OpenFadeSettings;
            settings.settings.useUnscaledTime = unscaledTime;
            settings.settings.duration = duration;
            settings.settings.startDelay = delay;
            var sequence = Sequence.Create(useUnscaledTime: unscaledTime);
            foreach (var target in targets)
                sequence.Group(PrimeTween.Tween.Alpha(target, settings.WithDirection(true, false)));
            if (callback != null) sequence.OnComplete(callback);
        }
        
        public static void OpenFadeTween(CanvasGroup target, bool unscaledTime, Action callback = null, float duration = 0.35f, float delay = 0)
        {
            var settings = OpenFadeSettings;
            settings.settings.useUnscaledTime = unscaledTime;
            settings.settings.duration = duration;
            settings.settings.startDelay = delay;
            var tween = PrimeTween.Tween.Alpha(target, settings.WithDirection(true, false));
            if (callback != null) tween.OnComplete(callback);
        }
        
        private static readonly TweenSettings<float> CloseFadeSettings = new()
        {
            startValue = 1f,
            endValue = 0f,
            settings = new TweenSettings()
            {
                ease = Ease.Default,
                duration = 0.35f,
            }
        };
        
        public static void CloseFadeTween(ICollection<Graphic> targets, bool unscaledTime, Action callback = null, float duration = 0.35f, float delay = 0)
        {
            var settings = CloseFadeSettings;
            settings.settings.useUnscaledTime = unscaledTime;
            settings.settings.duration = duration;
            settings.settings.startDelay = delay;
            var sequence = Sequence.Create(useUnscaledTime: unscaledTime);
            foreach (var target in targets)
                sequence.Group(PrimeTween.Tween.Alpha(target, settings.WithDirection(true, false)));
            if (callback != null) sequence.OnComplete(callback);
        }
        
        public static void CloseFadeTween(CanvasGroup target, bool unscaledTime, Action callback = null, float duration = 0.35f, float delay = 0)
        {
            var settings = CloseFadeSettings;
            settings.settings.useUnscaledTime = unscaledTime;
            settings.settings.duration = duration;
            settings.settings.startDelay = delay;
            var tween = PrimeTween.Tween.Alpha(target, settings.WithDirection(true, false));
            if (callback != null) tween.OnComplete(callback);
        }
        
        [Serializable]
        public class ScaleGroup
        {
            public Ease Ease = Ease.OutBack;
            [Min(0)] public float Delay;
            [Min(0)] public float Duration;
        
            public List<Transform> Elements = new ();
        }

        public static void OpenScaleGroupTween(ICollection<ScaleGroup> scaleGroups, bool unscaledTime, Action callback = null)
        {
            var sequence = Sequence.Create(useUnscaledTime: unscaledTime);
            foreach (var scaleGroup in scaleGroups)
            {
                foreach (var element in scaleGroup.Elements) 
                    element.localScale = Vector3.zero;
                
                foreach (var element in scaleGroup.Elements) 
                    sequence.Group(PrimeTween.Tween.Scale(element, Vector3.one, scaleGroup.Duration, scaleGroup.Ease, useUnscaledTime: unscaledTime, startDelay: scaleGroup.Delay));
            }
            if(callback != null) sequence.OnComplete(callback);
        }
    }
}