using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Projectile.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.View.Data;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Model
{
    public sealed class DefaultProjectileLauncher : BaseProjectileLauncher
    {
        public DefaultProjectileLauncher(ProjectileFactory projectileFactory, SkillViewFactory skillFactory, LocationFactory locationFactory, SkillViewData viewConfig) : base(projectileFactory, skillFactory, locationFactory, viewConfig)
        {
        }

        public override void Launch(float speed, Vector3 startPosition, Vector3 targetPosition)
        {
            var direction = targetPosition - startPosition;
            var projectile = CreateProjectile(startPosition, direction);
            projectile.Setup(speed, Vector3.Distance(startPosition, targetPosition));
            projectile.MoveByDirection(direction);
        }

        public override void Launch(float speed, Vector3 startPosition, Transform target)
        {
            var direction = target.position - startPosition;
            var projectile = CreateProjectile(startPosition, direction);
            projectile.Setup(speed);
            projectile.MoveTo(target, Vector3.zero);
        }
    }
}