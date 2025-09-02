using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.View
{
    public class BossRushWinWindow : ResourcedDungeonWinWindow<BossRushMode>
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);
    }
}