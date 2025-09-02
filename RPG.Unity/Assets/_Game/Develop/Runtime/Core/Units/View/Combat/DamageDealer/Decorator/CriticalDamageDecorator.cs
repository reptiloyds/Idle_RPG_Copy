using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.DamageDealer.Decorator
{
    public sealed class CriticalDamageDecorator : DamageDecorator
    {
        private UnitStat _criticalDamageChance;
        private UnitStat _criticalDamageMultiplier;
        
        private readonly Vector2 _criticalDamageChanceRange = new(0, 100);
        
        private readonly Color _color = Color.red;
        private readonly int _priority = 100;
        
        private bool _criticalDamageApplied;
        
        public CriticalDamageDecorator(UnitView unitView, IDamageProvider wrappedEntity) : base(unitView, wrappedEntity) => 
            UnitView.SpawnEvent += UpdateStats;

        private void UpdateStats()
        {
            ClearStats();
            
            _criticalDamageChance = UnitView.GetStat(UnitStatType.CriticalDamageChance);
            _criticalDamageMultiplier = UnitView.GetStat(UnitStatType.CriticalDamageMultiplier);
            _criticalDamageApplied = false;
            
            AppendStat(_criticalDamageChance);
            AppendStat(_criticalDamageMultiplier);
            
            UpdateReadyStatus();
        }

        protected override BigDouble.Runtime.BigDouble GetDamageInternal(UnitView targetUnitView)
        {
            var damage = WrappedEntity.GetDamage(targetUnitView);
            if (!IsReady) return damage;
            
            if (_criticalDamageChanceRange.Random() <= _criticalDamageChance.Value)
            {
                _criticalDamageApplied = true;
                damage *= _criticalDamageMultiplier.Value / 100;
            }
            else
                _criticalDamageApplied = false;
            
            return damage;  
        }

        protected override (Color color, int priority) GetColorInternal()
        {
            var tuple = base.GetColorInternal();
            if (_criticalDamageApplied && _priority > tuple.priority)
                return (_color, _priority);
            
            return base.GetColorInternal();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            UnitView.SpawnEvent -= UpdateStats;
            ClearStats();
        }
    }
}