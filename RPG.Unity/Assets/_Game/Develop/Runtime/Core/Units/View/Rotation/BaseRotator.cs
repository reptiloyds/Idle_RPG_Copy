using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Rotation
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class BaseRotator : UnitComponent
    {
        [SerializeField, Required] protected Transform _lookObject;
        [SerializeField, MinValue(0)] private float _sightAngel = 5f;
        [ShowInInspector, HideInEditorMode, ReadOnly] private float _angel; 
        
        protected Transform ObserveTarget;
        private bool _hasObserveTarget;
        private bool _isTargetInSight;
        
        public bool IsTargetInSight => _isTargetInSight;
        public event Action TargetInSight;
        public event Action TargetOutOfSight;
        public event Action LookCompleted;

        public void SetLookObject(Transform lookObject) => 
            _lookObject = lookObject;

        public virtual void StartObserve(Transform target)
        {
            ObserveTarget = target;
            _hasObserveTarget = true;
            _isTargetInSight = false;
        }

        public virtual void StopObserve()
        {
            ObserveTarget = null;
            _hasObserveTarget = false;
            _isTargetInSight = false;
        }

        public abstract void Look(Vector3 direction);
        
        public abstract void LookInstantly(Vector3 direction);
        
        private void Update()
        {
            if (!_hasObserveTarget) return;
            
            LookAtTarget();
        }
        
        protected void OnTargetInSight()
        {
            _isTargetInSight = true;
            TargetInSight?.Invoke();
        }

        protected void OnTargetOutOfSight()
        {
            _isTargetInSight = false;
            TargetOutOfSight?.Invoke();
        }

        protected void OnLookCompleted() => 
            LookCompleted?.Invoke();
        
        protected abstract void LookAtTarget();

        protected bool IsTransformInAngel(Transform lookObject)
        {
            var lookDirection = lookObject.forward;
            var targetDirection = (ObserveTarget.position - lookObject.position);
            targetDirection.y = 0;
            _angel = Vector3.Angle(lookDirection, targetDirection);
            
            return _angel <= _sightAngel;
        }
    }
}