using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.States;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Health
{
    [DisallowMultipleComponent]
    public class UnitHealthPresenter : UnitComponent
    {
        [SerializeField, Required] private Transform _healthBarPoint;
        [SerializeField, ReadOnly] private UnitData.Health _data;
        
        [ShowInInspector, HideInEditorMode]
        private UnitHealth _health;
        
        private bool _awaitDamage;
        private Tween _damageTween;

        public Transform HealthBarPoint => _healthBarPoint;

        public void SetupData(UnitData.Health data)
        {
            _data = data;
            ApplyData();
        }

        private void ApplyData() => 
            _healthBarPoint.transform.localPosition = Vector3.up * _data.ViewOffset;

        public void Set(UnitHealth health)
        {
            _health = health;
            _health.SetHideFlag(_data.HideWhenFull);
            _health.OnDamage += OnDamage;
            _health.OnHeal += OnHeal;
        }

        private void OnHeal(BigDouble.Runtime.BigDouble value)
        {
        }

        private void OnDamage(BigDouble.Runtime.BigDouble value)
        {
            if (_health.Value <= 0)
                Unit.StateMachine.SetSubState(SubStateType.Death);
            
            if (!_awaitDamage) return;
            if (_health.Value > 0)
                Unit.StateMachine.SetSubState(SubStateType.Damage);
        }

        public void Remove()
        {
            _health.OnDamage -= OnDamage;
            _health.OnHeal -= OnHeal;
            _health = null;
        }

        public void BeforeDamage() => 
            _awaitDamage = true;

        public void AfterDamage() => 
            _awaitDamage = false;

        public override void Dispose()
        {
            base.Dispose();
            
            _damageTween.Stop();
        }
    }
}