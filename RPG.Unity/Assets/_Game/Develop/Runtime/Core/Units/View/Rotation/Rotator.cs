using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Rotation
{
    public class Rotator : BaseRotator
    {
        [SerializeField, MinValue(0)] private float _rotationSpeed;
        
        private Tween _lookTween;

        public override void Look(Vector3 direction)
        {
            var flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
            var targetRotation = Quaternion.LookRotation(flatDirection);
            var angleY = Mathf.Abs(Mathf.DeltaAngle(_lookObject.eulerAngles.y, targetRotation.eulerAngles.y));
            
            var duration = angleY / _rotationSpeed;
            if (duration > 0)
            {
                _lookTween.Stop();
                _lookTween = Tween.Rotation(_lookObject, targetRotation, angleY / _rotationSpeed);
                _lookTween.OnComplete(OnLookCompleted);   
            }
            else
            {
                LookInstantly(direction);
                OnLookCompleted();
            }
        }

        public override void LookInstantly(Vector3 direction)
        {
            _lookTween.Stop();
            _lookObject.forward = direction;
        }

        public override void StartObserve(Transform target)
        {
            base.StartObserve(target);
            _lookTween.Stop();
        }

        protected override void LookAtTarget()
        {
            var direction = ObserveTarget.position - _lookObject.position;
            direction.y = 0;
            var targetRotation = Quaternion.LookRotation(direction);
            _lookObject.rotation = Quaternion.RotateTowards(_lookObject.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            
            
            var targetInSight = IsTransformInAngel(_lookObject);
            if (targetInSight && !IsTargetInSight)
                OnTargetInSight();
            else if (!targetInSight && IsTargetInSight) 
                OnTargetOutOfSight();
        }
    }
}