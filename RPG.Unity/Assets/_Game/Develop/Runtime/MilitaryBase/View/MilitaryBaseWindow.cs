using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.MilitaryBase.View
{
    public class MilitaryBaseWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private CanvasGroup _animationTarget;
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenFadeTween(_animationTarget, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseFadeTween(_animationTarget, UseUnscaledTime, callback);
    }
}