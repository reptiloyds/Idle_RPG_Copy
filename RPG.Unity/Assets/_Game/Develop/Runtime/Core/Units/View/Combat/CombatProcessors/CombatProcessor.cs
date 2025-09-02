using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.CombatProcessors
{
    public abstract class CombatProcessor : UnitComponent
    {
        [FormerlySerializedAs("UnitCombat")] [SerializeField, Required] protected Combat combat;

        protected override void GetComponents()
        {
            base.GetComponents();
            
            combat ??= GetComponentInParent<Combat>();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            
            combat.OnBeforeAttack += OnBeforeAttack;
            combat.OnAttackTrigger += OnAttackTrigger;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            combat.OnBeforeAttack -= OnBeforeAttack;
            combat.OnAttackTrigger -= OnAttackTrigger;
        }

        protected virtual void OnBeforeAttack(){}
        protected virtual void OnAttackTrigger(){}
    }
}