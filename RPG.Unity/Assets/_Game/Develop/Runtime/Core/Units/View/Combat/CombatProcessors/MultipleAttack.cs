using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.CombatProcessors
{
    public class MultipleAttack : CombatProcessor
    {
        private readonly List<(UnitStat chanceStat, int attackAmount)> _multipleAttackData = new();
        private readonly Vector2 _randomRange = new(0, 100);
        
        [ShowInInspector, HideInEditorMode, ReadOnly]
        private int _boostedAttackCount;

        public override void OnSpawn()
        {
            _multipleAttackData.Clear();
            
            var doubleAttack = Unit.GetStat(UnitStatType.DoubleAttackChance);
            if (doubleAttack != null) 
                _multipleAttackData.Add((doubleAttack, 2));
            
            var tripleAttack = Unit.GetStat(UnitStatType.TripleAttackChance);
            if (tripleAttack != null) 
                _multipleAttackData.Add((tripleAttack, 3));
            
            base.OnSpawn();
        }

        protected override void OnBeforeAttack()
        {
            base.OnBeforeAttack();
            
            if (_boostedAttackCount == 0) Roll();
            if (_boostedAttackCount > 0)
            {
                _boostedAttackCount--;
                Unit.AttackSpeed.LockMinimalAttackDelay();
            } 
        }

        protected override void OnAttackTrigger()
        {
            base.OnAttackTrigger();
            
            if (_boostedAttackCount == 0) 
                Unit.AttackSpeed.NormalizeAttackDelay();
        }

        private void Roll()
        {
            foreach (var tuple in _multipleAttackData)
            {
                if(!tuple.chanceStat.IsUnlocked) continue;
                if (_randomRange.Random() <= tuple.chanceStat.Value)
                {
                    _boostedAttackCount = tuple.attackAmount;
                    break;
                }
            }
        }
    }
}