using System;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.View
{
    public class GlobalBlessingWindow : BaseWindow
    {
        [SerializeField] private GlobalBlessingView _blessingView;
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;

        public void Setup(GlobalBlessing blessing) =>
            _blessingView.Setup(blessing);
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);
    }
}