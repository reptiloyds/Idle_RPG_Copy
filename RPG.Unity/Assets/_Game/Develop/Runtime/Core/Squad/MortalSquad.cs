using System;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Team;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Health;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Squad
{
    public abstract class MortalSquad : BaseSquad, ITickable
    {
        [Inject] private HealthBarFactory _healthBarFactory;
        [Inject] private MortalUnitContainer _mortalContainer;
        
        private SharedHealth _sharedHealth;
        private bool _healthIsShared;
        
        public event Action<UnitView> OnDie;
        public event Action OnAllUnitsDead;

        public override void Clear()
        {
            foreach (var unit in Units) 
                unit.OnDie -= OnUnitDie;
            
            base.Clear();
            
            if (_healthIsShared)
            {
                _sharedHealth.Dispose();
                _sharedHealth = null;
                _healthIsShared = false;
            }
        }

        public void FullHealth()
        {
            if (_healthIsShared)
                _sharedHealth.FullHealth();
            else
            {
                foreach (var unit in Units) 
                    unit.FullHealth();
            }
        }
        
        public void MergeHealth()
        {
            _healthIsShared = true;
            var health = StatService.GetPlayerStat(UnitStatType.Health);
            var regen = StatService.GetPlayerStat(UnitStatType.HealthRegenAmount);
            regen.SetModifierReference(health);
            var regenSpeed = StatService.GetPlayerStat(UnitStatType.HealthRegenSpeed);
            _sharedHealth = new SharedHealth(_healthBarFactory, health, regen, regenSpeed); // todo move to ally squad
            _sharedHealth.Initialize();
            foreach (var unit in Units)
            {
                if (unit.Health != null)
                {
                    unit.Health.Dispose();
                    unit.RemoveHealth();
                }
                _sharedHealth.Setup(unit); 
                unit.SetHealth(_sharedHealth);
            } 
        }

        public void SplitHealth()
        {
            _healthIsShared = false;
            foreach (var unit in Units)
            {
                _sharedHealth.Remove(unit);
                unit.RemoveHealth();
                CreateOwnHealth(unit);
            }
            _sharedHealth.Dispose();
            _sharedHealth = null;
        }
        
        private void CreateOwnHealth(UnitView unitView)
        {
            var health = new OwnHealth(_healthBarFactory, unitView);
            health.Initialize();
            unitView.SetHealth(health);
        }

        protected override void AppendUnit(UnitView unitView)
        {
            if (_healthIsShared)
            {
                _sharedHealth.Setup(unitView);
                unitView.SetHealth(_sharedHealth);
            }
            else
                CreateOwnHealth(unitView);
            
            base.AppendUnit(unitView);
            
            unitView.OnDie += OnUnitDie;
            _mortalContainer.Add(unitView);
        }

        protected override void RemoveUnit(UnitView unitView)
        {
            if (_healthIsShared)
            {
                _sharedHealth.Remove(unitView);
                unitView.RemoveHealth();
            }
            else
            {
                unitView.Health.Dispose();
                unitView.RemoveHealth();
            }
            
            _mortalContainer.Remove(unitView);
            base.RemoveUnit(unitView);
        }

        protected virtual void OnUnitDie(UnitView unitView)
        {
            unitView.OnDie -= OnUnitDie;
            OnDie?.Invoke(unitView);
            RemoveUnit(unitView);
            if (Units.Count == 0) 
                OnAllUnitsDead?.Invoke();
        }

        public void Tick()
        {
            if(_healthIsShared)
                _sharedHealth.Tick();
        }
    }
}