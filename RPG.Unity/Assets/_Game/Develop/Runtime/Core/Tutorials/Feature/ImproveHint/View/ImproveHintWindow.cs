using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.ImproveHint.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ImproveHintWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private WindowTweens.ScaleGroup[] _scaleGroups;
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleGroupTween(_scaleGroups, UseUnscaledTime, callback);
    }
}