using PleasantlyGames.RPG.Runtime.Core.Projectile.View;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Projectile.Model.Projectile.State
{
    public class ProjectileFollowTransform : ProjectileState
    {
        private bool _lookToTarget;
        private Transform _target;
        private Vector3 _offset;
        
        private const float CancelAutoRotationDistance = 0.1f;
        
        public ProjectileFollowTransform(BaseProjectile baseProjectile) : base(baseProjectile)
        {
        }

        public void Setup(Transform target, Vector3 offset)
        {
            _target = target;
            _offset = offset;
        }

        public override void Enter()
        {
            base.Enter();
            
            var targetPos = _target.transform.position + _offset;
            BaseProjectile.Forward(targetPos - BaseProjectile.transform.position);
            
            _lookToTarget = !(Vector3.Distance(BaseProjectile.transform.position, _target.transform.position + _offset) <= CancelAutoRotationDistance);
        }

        public override void Update()
        {
            base.Update();
            
            if (!_target.gameObject.activeInHierarchy) 
                InvokeComplete();
            
            if (_lookToTarget)
            {
                var targetPos = _target.transform.position + _offset;
                if(Vector3.Distance(BaseProjectile.transform.position, targetPos) <= CancelAutoRotationDistance)
                    _lookToTarget = false;
                else
                    BaseProjectile.Forward(targetPos - BaseProjectile.transform.position);
            }
            
            BaseProjectile.Move(BaseProjectile.transform.forward);
        }
    }
}