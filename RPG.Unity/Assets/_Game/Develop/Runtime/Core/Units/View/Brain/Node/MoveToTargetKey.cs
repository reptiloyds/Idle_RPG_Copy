using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node
{
    internal class MoveToTargetKey : UnitNode
    {
        private Vector3 _targetPosition;
        private Vector3 _lookDirection;
        private bool _hasTargetPosition;
        private bool _hasLookDirection;

        private bool _completeOnNextFrame;
        
        private const float DestinationReachedDistance = 0.05f;
        
        public MoveToTargetKey(UnitView unitView) : base(unitView)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _completeOnNextFrame = false;
            
            var positionResult = UnitView.GetPosition(UnitView.TargetKey);
            _targetPosition = positionResult.position;
            _hasTargetPosition = positionResult.success;
            
            var lookDirectionResult = UnitView.GetLookDirection(UnitView.TargetKey);
            _lookDirection = lookDirectionResult.direction;
            _hasLookDirection = lookDirectionResult.success;
            
            if (!_hasTargetPosition || Vector3.Distance(UnitView.transform.position, _targetPosition) <= DestinationReachedDistance)
            {
                if (_hasLookDirection)
                {
                    // Unit.Watcher.LookCompleted += OnLookComplete;
                    UnitView.Rotator.Look(_lookDirection);
                }
                //else
                CompleteOnNextFrame();
            }
            else
            {
                UnitView.BaseMovement.OnDestinationReached += OnDestinationReached;
                UnitView.BaseMovement.MoveTo(_targetPosition);
            }
        }

        public override void Exit()
        {
            base.Exit();
            
            UnitView.BaseMovement.OnDestinationReached -= OnDestinationReached;
            UnitView.Rotator.LookCompleted -= OnLookComplete;
        }

        private void OnDestinationReached()
        {
            if (Vector3.Distance(UnitView.transform.position, _targetPosition) > DestinationReachedDistance)
            {
                UnitView.BaseMovement.MoveTo(_targetPosition);
                return;
            }

            if (_hasLookDirection)
            {
                //Unit.Watcher.LookCompleted += OnLookComplete;
                UnitView.Rotator.Look(_lookDirection);
            }
            //else
            CompleteOnNextFrame();
        }

        private void OnLookComplete() => 
            CompleteOnNextFrame();

        private void CompleteOnNextFrame() => 
            _completeOnNextFrame = true;

        public override void Update()
        {
            base.Update();

            if (!_completeOnNextFrame) return;
            _completeOnNextFrame = false;
            Complete();
        }
    }
}