using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Animations.Old
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class WeaponAnimator : MonoBehaviour
    {
        protected float AttackSpeedK = 1;
        
        public event Action OnShoot;

        public void SetAttackSpeedK(float attackSpeedK) => 
            AttackSpeedK = attackSpeedK;
        
        public abstract void Play();
        
        protected void ExecuteOnShoot() => 
            OnShoot?.Invoke();
    }
}