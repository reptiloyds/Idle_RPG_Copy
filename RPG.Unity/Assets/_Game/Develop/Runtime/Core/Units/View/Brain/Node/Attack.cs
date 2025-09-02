using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.NodeMachine.Model;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node
{
    internal class Attack : UnitNode
    {
        private readonly VariableContainer<UnitView> _target;
        private UnitView _targetUnitView;

        private const float CheckDistanceDelay = 0.5f;
        private float _nextCheckDistanceTime;

        private float _attackRange;
        private float _fullAttackRange;

        private const float _offsetInaccuracy = 0.075f;

        private Transform _movementTarget;

        public Attack(UnitView unitView, VariableContainer<UnitView> target) : base(unitView) =>
            _target = target;

        public override void Enter()
        {
            base.Enter();

            var unit = _target.Get();
            if (unit.IsDead)
            {
                Fail();
                return;
            }

            _targetUnitView = unit;

            _attackRange = (float)UnitView.GetStat(UnitStatType.AttackRange).Value.ToDouble() + UnitView.Radius;
            _fullAttackRange = _attackRange + _targetUnitView.Radius;

            _attackRange += _offsetInaccuracy;
            _fullAttackRange += _offsetInaccuracy;

            _movementTarget = _targetUnitView.GetTargetForEnemy(UnitView.gameObject);

            _targetUnitView.OnDie += UnitViewDispose;
            _targetUnitView.DisposeEvent += UnitViewDispose;

            UnitView.Rotator.TargetInSight += OnTargetInSight;
            UnitView.Rotator.TargetOutOfSight += OnTargetOutOfSight;
            UnitView.Rotator.StartObserve(_targetUnitView.ProjectileTarget);

            ResetCheckDistanceTime();
        }

        private void OnTargetInSight() => 
            UnitView.Targets.Set(_targetUnitView);

        private void OnTargetOutOfSight() => 
            UnitView.Targets.Clear();

        public override void Exit()
        {
            base.Exit();

            UnitView.Rotator.TargetInSight -= OnTargetInSight;
            UnitView.Rotator.TargetOutOfSight -= OnTargetOutOfSight;
            UnitView.Targets.Clear();
            UnitView.Rotator.StopObserve();

            if (_targetUnitView != null)
            {
                _targetUnitView.ReleaseTargetForEnemy(UnitView.gameObject);
                _targetUnitView.OnDie -= UnitViewDispose;
                _targetUnitView = null;
            }

            _target.Set(null);
        }

        public override void Update()
        {
            base.Update();

            if (Time.time < _nextCheckDistanceTime) return;

            ResetCheckDistanceTime();

            if (_movementTarget == null)
            {
                if (_targetUnitView != null)
                {
                    if (Vector3.Distance(UnitView.transform.position, _targetUnitView.transform.position) > _fullAttackRange)
                        Fail();   
                }
            }
            else
            {
                if (Vector3.Distance(UnitView.transform.position, _movementTarget.position) > _attackRange)
                    Fail();
            }
        }

        private void ResetCheckDistanceTime() =>
            _nextCheckDistanceTime = Time.time + CheckDistanceDelay;

        private void UnitViewDispose(UnitView unitView) =>
            Complete();
    }
}