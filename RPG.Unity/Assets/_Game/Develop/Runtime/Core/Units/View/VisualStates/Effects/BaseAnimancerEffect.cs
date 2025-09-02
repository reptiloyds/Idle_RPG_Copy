using PleasantlyGames.RPG.Runtime.Core.Units.View.Animations;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Building;
using UnityEngine;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects
{
    public abstract class BaseAnimancerEffect : VisualEffect, IUpdateableBuildElement
    {
        [SerializeField] private AnimancerController _controller;
        
        public AnimancerController Controller => _controller;

        void IUpdateableBuildElement.UpdateState(UnitView unitView)
        {
            if(_controller == null)
                _controller = unitView.transform.GetComponentInParent<AnimancerController>();
            if(_controller == null)
                _controller = unitView.transform.GetComponentInChildren<AnimancerController>();   
        }

        void IBuildElement.LogIfWrong(ref int errorCount)
        {
            if (_controller != null) return;
            errorCount++;
            Logger.LogError($"No {nameof(AnimancerController)} found on the unit", gameObject);
        }
    }
}