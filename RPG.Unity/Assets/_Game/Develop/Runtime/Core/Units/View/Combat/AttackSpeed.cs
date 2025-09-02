using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat
{
    public class AttackSpeed : UnitComponent
    {
        [SerializeField, MinValue(0.1f)] private float _minimalAttackDelay = 0.1f;
        
        public readonly ReactiveProperty<float> Delay = new();

        private bool _attackDelayIsMinimal;
        private UnitStat _attackSpeed;
        private float _attackSpeedK;
        
        public override void OnSpawn()
        {
            base.OnSpawn();
            
            _attackSpeed = Unit.GetStat(UnitStatType.AttackSpeed);
            _attackSpeed.OnValueChanged += OnAttackSpeedChanged;
            RecalculateAttackDelay();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _attackSpeed.OnValueChanged -= OnAttackSpeedChanged;
        }
        
        public void LockMinimalAttackDelay()
        {
            _attackDelayIsMinimal = true;
            RecalculateAttackDelay();
        }

        public void NormalizeAttackDelay()
        {
            _attackDelayIsMinimal = false;
            RecalculateAttackDelay();
        }
        
        private void OnAttackSpeedChanged() => 
            RecalculateAttackDelay();
        
        private void RecalculateAttackDelay()
        {
            if (_attackDelayIsMinimal) 
                Delay.Value = _minimalAttackDelay;
            else
                Delay.Value = 1 / (float)_attackSpeed.Value.ToDouble();
        }
    }
}