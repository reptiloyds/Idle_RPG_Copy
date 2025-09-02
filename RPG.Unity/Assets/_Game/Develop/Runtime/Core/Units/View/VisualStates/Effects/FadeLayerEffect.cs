using PleasantlyGames.RPG.Runtime.Core.Units.View.Animations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects
{
    public class FadeLayerEffect : BaseAnimancerEffect
    {
        [SerializeField] private AnimancerLayerType _layerType;
        [SerializeField, Range(0, 1)] private float _activeValue;
        [SerializeField, MinValue(0)] private float _activeValueDuration;
        [SerializeField, Range(0, 1)] private float _inactiveValue;
        [SerializeField, MinValue(0)] private float _inactiveValueDuration;
        
        public override void Activate(UnitView unitView)
        {
            base.Activate(unitView);
            
            Controller.FadeLayer(_layerType, _activeValue, _activeValueDuration);
        }

        public override void Deactivate(UnitView unitView)
        {
            base.Deactivate(unitView);
            
            Controller.FadeLayer(_layerType, _inactiveValue, _inactiveValueDuration);
        }
    }
}