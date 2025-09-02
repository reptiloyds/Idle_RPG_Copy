using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GuideWindows
{
    public class SoftRushGuideWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")]
        private RectTransform[] _animationTargets;
        
        
        protected override void OpenAnimation(Action callback) =>
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) =>
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);
    }
}