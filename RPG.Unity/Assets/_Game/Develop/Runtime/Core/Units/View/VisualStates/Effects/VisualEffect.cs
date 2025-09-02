using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects
{
    [HideMonoScript]
    public abstract class VisualEffect : MonoBehaviour
    {
        protected UnitView CurrentUnitView;

        public virtual void OnSpawn()
        {
            
        }

        public virtual void Activate(UnitView unitView) => 
            CurrentUnitView = unitView;

        public virtual void Deactivate(UnitView unitView) => 
            CurrentUnitView = unitView;

        private void OnDestroy()
        {
            if(CurrentUnitView != null)
                Deactivate(CurrentUnitView);
        }
    }
}