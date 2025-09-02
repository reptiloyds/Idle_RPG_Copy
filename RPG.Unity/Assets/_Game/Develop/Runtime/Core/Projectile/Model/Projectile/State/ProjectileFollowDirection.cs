using PleasantlyGames.RPG.Runtime.Core.Projectile.View;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Projectile.Model.Projectile.State
{
    public class ProjectileFollowDirection : ProjectileState
    {
        private Vector3 _targetDirection;
        
        public ProjectileFollowDirection(BaseProjectile baseProjectile) : base(baseProjectile)
        {
        }

        public void Setup(Vector3 targetDirection)
        {
            _targetDirection = targetDirection;
        }

        public override void Enter()
        {
            base.Enter();
            
            BaseProjectile.Forward(_targetDirection);
        }

        public override void Update()
        {
            base.Update();
            
            BaseProjectile.Forward(_targetDirection);
            BaseProjectile.Move(BaseProjectile.transform.forward);
        }
    }
}