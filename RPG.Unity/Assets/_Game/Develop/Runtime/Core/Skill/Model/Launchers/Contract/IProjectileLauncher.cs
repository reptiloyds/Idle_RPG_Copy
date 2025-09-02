using System;
using PleasantlyGames.RPG.Runtime.Core.Projectile.View;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Contract
{
    public interface IProjectileLauncher
    {
        event Action<BaseProjectile, Collider> ProjectileTriggerEnter;  
        event Action<BaseProjectile> ProjectileComplete;
        void Launch(float speed, Vector3 startPosition, Vector3 targetPosition);
        void Launch(float speed, Vector3 startPosition, Transform target);
        void ReleaseAll(bool instantDespawn = false);
        void ReleaseProjectile(BaseProjectile projectile, bool instantDespawn = false);
        void FixedTick();
    }
}