using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.NodeMachine.Model;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node
{
    internal class FollowForAttack : UnitNode
    {
        private readonly VariableContainer<UnitView> _target;
        private readonly Movement.BaseMovement _baseMovement;
        private readonly float _updatePathDelay;
        private const float _movementOffsetInaccuracy = 0.05f;

        private float _attackRange;
        private float _fullAttackRange;
        
        private bool _isFollowing = false;
        private float _updatePathTime;
        private UnitView _targetUnitView;

        private Transform _movementTarget;

        public FollowForAttack(UnitView unitView, VariableContainer<UnitView> target, float updatePathDelay) : base(unitView)
        {
            _baseMovement = UnitView.BaseMovement;
            _target = target;
            _updatePathDelay = updatePathDelay;
        }

        public override void Enter()
        {
            base.Enter();

            _targetUnitView = _target.Get();
            _attackRange = (float)UnitView.GetStat(UnitStatType.AttackRange).Value.ToDouble() + UnitView.Radius;
            _fullAttackRange = _attackRange + _targetUnitView.Radius;

            if (Vector3.Distance(UnitView.transform.position, _targetUnitView.transform.position) < _fullAttackRange)
            {
                Complete();
                return;
            }

            _movementTarget = _targetUnitView.GetTargetForEnemy(UnitView.gameObject);

            _targetUnitView.OnDie += OnTargetDie;
            _isFollowing = true;
            Follow();
        }

        public override void Exit()
        {
            base.Exit();

            if (_isFollowing)
            {
                _isFollowing = false;
                _baseMovement.Stop();
            }
            
            _targetUnitView.OnDie -= OnTargetDie;
            _targetUnitView = null;
        }

        private void OnTargetDie(UnitView unitView) =>
            Fail();

        private void Follow()
        {
            _updatePathTime = Time.time + _updatePathDelay;
            if (_movementTarget == null)
                _baseMovement.MoveTo(_targetUnitView.transform.position, _fullAttackRange - _movementOffsetInaccuracy);
            else
                _baseMovement.MoveTo(_movementTarget.position, _attackRange - _movementOffsetInaccuracy);
        }

        public override void Update()
        {
            base.Update();
            if (!_isFollowing) return;

            if (_movementTarget == null)
            {
                if (Vector3.Distance(UnitView.transform.position, _targetUnitView.transform.position) < _fullAttackRange)
                    Complete();
            }
            else
            {
                if (Vector3.Distance(UnitView.transform.position, _movementTarget.position) < _attackRange)
                    Complete();
            }

            if (Time.time < _updatePathTime) return;
            Follow();
        }
    }
}