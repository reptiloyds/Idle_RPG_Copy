using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PrimeTween;
using UnityEngine;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Health
{
    public class SharedHealth : UnitHealth, ITickable
    {
        private readonly UnitStat _health;
        private readonly UnitStat _regen;
        private readonly UnitStat _regenSpeed;

        private readonly List<UnitView> _units = new();
        private bool _isDisposed;
        private Sequence _regenSequence;

        public SharedHealth(HealthBarFactory healthBarFactory, UnitStat health, UnitStat regen, UnitStat regenSpeed) : base(healthBarFactory)
        {
            _health = health;
            _regen = regen;
            _regenSpeed = regenSpeed;
            _health.OnValueChanged += RecalculateHealth;
            TryAddHealth(_health.Value);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            Bar = HealthBarFactory.Create(CalculateAverageUnitPosition(), this);
        }

        public float GetRegenTime()
        {
            if (_regenSpeed != null) return (float)_regenSpeed.Value.ToDouble();
            return 1;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            HealthBarFactory.Dispose(Bar);
            _regenSequence.Stop();
            _isDisposed = true;
        }

        public void Setup(UnitView unitView)
        {
            _units.Add(unitView);
            
            RecalculateHealth();
            RebuildRegenSequence();
            
            // _healthBar.SetPosition(CalculateAverageUnitPosition());
        }

        public void Remove(UnitView unitView)
        {
            _units.Remove(unitView);
            
            RecalculateHealth();
            RebuildRegenSequence();
            
            if(_units.Count == 0 && Bar != null)
                Bar.gameObject.SetActive(false);
            // _healthBar.SetPosition(CalculateAverageUnitPosition());
        }

        public void Tick()
        {
            if(_isDisposed) return;
            Bar.SetPosition(CalculateAverageUnitPosition());
        }

        private void RecalculateHealth()
        {
            SetNewMaxValue(_health.Value);
        }

        private void RebuildRegenSequence()
        {
            _regenSequence.Stop();
            if (_units.Count == 0) return;
            
            _regenSequence = Sequence.Create(-1);
            _regenSequence.Chain(Tween.Delay(GetRegenTime(), Regen));
        }

        private void Regen()
        {
            ApplyHeal(_regen.Value);
        }

        private void TryAddHealth(BigDouble.Runtime.BigDouble value)
        {
            Value += value;
            if (Value > MaxValue) 
                Value = MaxValue;
        }

        private Vector3 CalculateAverageUnitPosition()
        {
            if(_units.Count == 0) return Vector3.zero;
            
            float sumX = 0;
            float sumY = 0;
            float sumZ = 0;
            
            foreach (var unit in _units)
            {
                var healthBarPosition = unit.HealthBarPoint.position;
                sumX += healthBarPosition.x;
                sumY += healthBarPosition.y;
                sumZ += healthBarPosition.z;
            }
            
            var averageX = sumX / _units.Count;
            var averageY = sumY / _units.Count;
            var averageZ = sumZ / _units.Count;
            
            return new Vector3(averageX, averageY, averageZ);
        }
    }
}