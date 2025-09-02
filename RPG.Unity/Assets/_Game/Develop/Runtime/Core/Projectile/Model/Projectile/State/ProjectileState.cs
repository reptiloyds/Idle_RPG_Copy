using System;
using PleasantlyGames.RPG.Runtime.Core.Projectile.View;

namespace PleasantlyGames.RPG.Runtime.Core.Projectile.Model.Projectile.State
{
    public abstract class ProjectileState
    {
        protected readonly BaseProjectile BaseProjectile;

        public event Action Complete;
            
        protected ProjectileState(BaseProjectile baseProjectile) => 
            BaseProjectile = baseProjectile;

        public virtual void Enter()
        {
            
        }

        public virtual void Exit()
        {
            
        }

        public virtual void Update()
        {
            
        }

        protected void InvokeComplete() => 
            Complete?.Invoke();
    }
}