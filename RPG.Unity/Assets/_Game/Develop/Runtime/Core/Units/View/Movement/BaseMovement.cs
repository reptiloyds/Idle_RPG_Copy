using System;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.States;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Movement
{
    [DisallowMultipleComponent]
    public abstract class BaseMovement : UnitComponent
    {
        [SerializeField, ReadOnly] private UnitData.Movement _data;
        
        [ShowInInspector, HideInEditorMode] protected Vector3 DestinationPoint;
        [ShowInInspector, HideInEditorMode] protected bool IsMoving;

        private bool _fakeMovementIsActive;
        private Vector3 _originalModelPosition;
        
        private Tween _fakeMovement;
        private const float StopDistance = 0.1f;
        protected UnitStat SpeedStat;

        public MovementType Type => _data.Type;
        public abstract float Radius { get; }
        public abstract float Height { get; }
        
        public event Action OnStop;
        public event Action OnDestinationReached;

        public abstract void SetPosition(Vector3 position);
        public abstract void SetRotation(Vector3 direction);

        public void SetupData(UnitData.Movement data) => 
            _data = data;

        public virtual void MoveTo(Vector3 position, float offset = 0)
        {
            var directionToUnit = (Unit.transform.position - position).normalized;
            DestinationPoint = position + directionToUnit * offset;
            
            if (IsMoving) return;
            IsMoving = true;
            Unit.StateMachine.SetSubState(SubStateType.Move);
        }

        public void StartFakeMove()
        {
            if(!_data.AnimateFakeMovement) return;
            _fakeMovementIsActive = true;
            _fakeMovement.Stop();
            _data.StartFakeMovement.startFromCurrent = true;
            _originalModelPosition = Unit.Visual.transform.localPosition;
            _fakeMovement = Tween.LocalPositionZ(Unit.Visual.transform, _data.StartFakeMovement);
        }

        public void StopFakeMove()
        {
            if(!_data.AnimateFakeMovement) return;
            _fakeMovementIsActive = false;
            _fakeMovement.Stop();
            _data.StopFakeMovement.startFromCurrent = true;
            _fakeMovement = Tween.LocalPositionZ(Unit.Visual.transform, _data.StopFakeMovement);
        }

        public void ResetFakeMove()
        {
            if(_fakeMovementIsActive) return;
            
            _fakeMovement.Stop();
            Unit.Visual.transform.localPosition = _originalModelPosition;
        }

        public virtual void Stop()
        {
            IsMoving = false;
            Unit.StateMachine.SetSubState(SubStateType.Idle);
            OnStop?.Invoke();
        }

        protected virtual void Update()
        {
            if (IsMoving && IsDistanceReached())
            {
                Stop();
                OnDestinationReached?.Invoke();
            }
        }

        protected virtual bool IsDistanceReached() => 
            Vector3.Distance(Unit.transform.position, DestinationPoint) <= StopDistance;

        public override void OnSpawn()
        {
            base.OnSpawn();
            
            SpeedStat = Unit.GetStat(UnitStatType.MoveSpeed);
            SpeedStat.OnValueChanged += OnUpdateSpeed;
            OnUpdateSpeed();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            SpeedStat.OnValueChanged -= OnUpdateSpeed;
            IsMoving = false;
        }

        protected abstract void OnUpdateSpeed();
    }
}