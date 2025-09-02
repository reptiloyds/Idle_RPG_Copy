using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Model
{
    public class PowerCalculator : IDisposable
    {
        private BigDouble.Runtime.BigDouble _power;
        
        private readonly List<UnitStat> _stats = new();
        
        private UnitStat _damage;
        private UnitStat _attackSpeed;
        private UnitStat _criticalMultiplier;
        private UnitStat _doubleAttackChance;
        private UnitStat _tripleAttackChance;
        private UnitStat _advancedDamage;
        private UnitStat _normalEnemyDamage;
        
        public event Action<BigDouble.Runtime.BigDouble, BigDouble.Runtime.BigDouble> OnPowerChanged;
        public readonly ReactiveProperty<BigDouble.Runtime.BigDouble> Power = new();

        [Preserve]
        public PowerCalculator() { }

        public void Setup(IReadOnlyList<ImprovableUnitStat> stats)
        {
            foreach (var stat in stats)
            {
                switch (stat.Type)
                {
                    case UnitStatType.Damage:
                        _damage = stat;
                        _stats.Add(stat);
                        break;
                    case UnitStatType.AttackSpeed:
                        _attackSpeed = stat;
                        _stats.Add(stat);
                        break;
                    case UnitStatType.CriticalDamageMultiplier:
                        _criticalMultiplier = stat;
                        _stats.Add(stat);
                        break;
                    case UnitStatType.DoubleAttackChance:
                        _doubleAttackChance = stat;
                        _stats.Add(stat);
                        break;
                    case UnitStatType.TripleAttackChance:
                        _tripleAttackChance = stat;
                        _stats.Add(stat);
                        break;
                    case UnitStatType.AdvancedDamage:
                        _advancedDamage = stat;
                        _stats.Add(stat);
                        break;
                    case UnitStatType.NormalEnemyDamage:
                        _normalEnemyDamage = stat;
                        _stats.Add(stat);
                        break;
                }
            }

            foreach (var powerStat in _stats) 
                powerStat.OnValueChanged += OnStatValueChanged;

            RecalculatePower();   
        }
        
        private void OnStatValueChanged()
        {
            var lastPower = _power;
            RecalculatePower();
            OnPowerChanged?.Invoke(lastPower, _power);
        }

        private void RecalculatePower()
        {
            var power = _damage.StableValue;
            if(_attackSpeed is { IsUnlocked: true })
                power *= _attackSpeed.StableValue;
            if(_criticalMultiplier is { IsUnlocked: true })
                power *= _criticalMultiplier.StableValue / 100;
            if (_doubleAttackChance is { IsUnlocked: true }) 
                power *= 1 + _doubleAttackChance.StableValue / 100;
            if (_tripleAttackChance is { IsUnlocked: true }) 
                power *= 1 + _tripleAttackChance.StableValue / 100;
            if (_advancedDamage is { IsUnlocked: true }) 
                power *= 1 + _advancedDamage.StableValue / 100;
            if (_normalEnemyDamage is { IsUnlocked: true }) 
                power *= 1 + _normalEnemyDamage.StableValue / 100;

            Power.Value = power;
        }

        public void Dispose()
        {
            foreach (var powerStat in _stats) 
                powerStat.OnValueChanged -= OnStatValueChanged;
        }
    }
}