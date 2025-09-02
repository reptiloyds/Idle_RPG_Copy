using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Units;
using PleasantlyGames.RPG.Runtime.Core.Units.Factory;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Squad
{
    public abstract class BaseSquad : IDisposable
    {
        [Inject] protected UnitFactory Factory;
        [Inject] protected UnitStatService StatService;
        [Inject] protected BalanceContainer Balance;

        protected string LastMovementTarget;
        protected UnitSpawnProvider SpawnProvider;
        
        public event Action OnAchieveMovementTarget;
        
        protected UnitBehaviourType BehaviourType { get; private set; }
        protected readonly List<UnitView> Units = new();

        public abstract TeamType TeamType { get; }

        public List<UnitView> GetUnits() => Units;

        [Preserve]
        protected BaseSquad()
        {
            
        }
        
        public virtual void Initialize() { }

        public void SetSpawn(UnitSpawnProvider spawnProvider) => 
            SpawnProvider = spawnProvider;

        public virtual void Clear()
        {
            for (int i = Units.Count -1; i >= 0; i--)
            {
                var unit = Units[i];
                unit.OnAchieveTarget -= OnAchieveTarget;
                RemoveUnit(unit);
            }

            BehaviourType = UnitBehaviourType.None;
            SpawnProvider = null;
        }

        protected virtual void AppendUnit(UnitView unitView)
        {
            Units.Add(unitView);
            if (BehaviourType == UnitBehaviourType.None) return;
            
            switch (BehaviourType)
            {
                case UnitBehaviourType.MoveToTarget:
                    SetupMovementTarget(unitView, LastMovementTarget);
                    break;
            }
            unitView.SetBehaviour(BehaviourType);
        }

        protected virtual void RemoveUnit(UnitView unitView)
        {
            SpawnProvider.Free(unitView.gameObject);
            Units.Remove(unitView);
            Factory.Release(unitView);
        }

        protected virtual async UniTask<UnitView> SpawnUnitAsync(string unitId, UnitSpawnPoint spawnPoint, int evolution = 0)
        {
            var unit = await Factory.CreateAsync(unitId, evolution);
            var point = spawnPoint.Point.position;
            unit.SetTeamType(TeamType);
            unit.SetPosition(point);
            unit.SetRotation(SpawnProvider.LookDirection);
            unit.AppendPosition(UnitPointKey.Spawn, point);
            unit.AppendLookDirection(UnitPointKey.Spawn, SpawnProvider.LookDirection);
            SpawnProvider.Occupy(unit.gameObject, spawnPoint);
            
            return unit;
        }

        public void AddModifier(UnitStatType statType, StatModifier statModifier)
        {
            foreach (var unit in Units) 
                ApplyModifier(unit, statType, statModifier);
        }

        public void RemoveModifier(UnitStatType statType, StatModifier statModifier)
        {
            foreach (var unit in Units) 
                CancelModifier(unit, statType, statModifier);
        }

        protected void ApplyModifier(UnitView unitView, UnitStatType statType, StatModifier statModifier)
        {
            var stat = unitView.GetStat(statType);
            stat.AddModifier(statModifier);
        }
        
        protected void CancelModifier(UnitView unitView, UnitStatType statType, StatModifier statModifier)
        {
            var stat = unitView.GetStat(statType);
            stat.RemoveModifier(statModifier);
        }

        public virtual void SimulateMovement() => SetBehaviour(UnitBehaviourType.SimulateMovement);

        public virtual void MoveForward() => SetBehaviour(UnitBehaviourType.MoveForward);

        public virtual void Fight() => SetBehaviour(UnitBehaviourType.Fight);

        public virtual void MoveToSpawn(string targetKey)
        {
            LastMovementTarget = targetKey;
            foreach (var unit in Units) 
                SetupMovementTarget(unit, targetKey);
            SetBehaviour(UnitBehaviourType.MoveToTarget);
        }
        
        public bool HasAnyMovementTarget()
        {
            foreach (var unit in Units)
                if (unit.HasTargetKey) return true;
            return false;
        }

        public virtual void Idle() => SetBehaviour(UnitBehaviourType.Idle);

        private void SetBehaviour(UnitBehaviourType type)
        {
            BehaviourType = type;
            foreach (var unit in Units) 
                unit.SetBehaviour(BehaviourType);
        }

        protected void SetupMovementTarget(UnitView unitView, string targetKey)
        {
            unitView.SetupTargetKey(targetKey);
            unitView.OnAchieveTarget += OnAchieveTarget;
        }

        private void OnAchieveTarget(UnitView unitView)
        {
            unitView.OnAchieveTarget -= OnAchieveTarget;

            if (!HasAnyMovementTarget()) 
                OnAchieveMovementTarget?.Invoke();
        }

        public virtual void Dispose()
        {
            
        }
    }
}