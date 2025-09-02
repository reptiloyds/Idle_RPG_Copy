using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.Weapons
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] private List<VisualStates.Effects.VisualEffect> _idleEffects;
        [SerializeField] private List<VisualStates.Effects.VisualEffect> _attackEffects;
        
        public IReadOnlyList<VisualStates.Effects.VisualEffect> IdleEffects => _idleEffects;
        public IReadOnlyList<VisualStates.Effects.VisualEffect> AttackEffects => _attackEffects;
        
        public abstract WeaponType Type { get; }
        public event Action<UnitView> OnAttackPerformed;

        protected TeamType TeamType { get; private set; }

        public void SetTeamType(TeamType teamType) => 
            TeamType = teamType;

        public abstract void PerformAttack(UnitView target);
        public abstract void CancelAttack();
        
        protected void AttackPerformedTo(UnitView target) =>
            OnAttackPerformed?.Invoke(target);
    }
}