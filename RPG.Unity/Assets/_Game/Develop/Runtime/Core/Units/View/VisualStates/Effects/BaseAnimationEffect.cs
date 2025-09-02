using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Building;
using PleasantlyGames.RPG.Runtime.TweenUtilities;
using UnityEngine;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects
{
    public class BaseAnimationEffect : VisualEffect, IBuildElement
    {
        [SerializeField] private BaseAnimation[] _animations;

        void IBuildElement.LogIfWrong(ref int errorCount)
        {
            if (_animations.Length == 0 || _animations.Any(item => item == null))
            {
                errorCount++;
                Logger.LogError($"Animations are not set on {nameof(BaseAnimationEffect)}", gameObject);
            }
        }

        public override void Activate(UnitView unitView)
        {
            base.Activate(unitView);

            foreach (var baseAnimation in _animations) 
                baseAnimation.Play();
        }

        public override void Deactivate(UnitView unitView)
        {
            base.Deactivate(unitView);
            
            foreach (var baseAnimation in _animations) 
                baseAnimation.Stop();
        }
    }
}