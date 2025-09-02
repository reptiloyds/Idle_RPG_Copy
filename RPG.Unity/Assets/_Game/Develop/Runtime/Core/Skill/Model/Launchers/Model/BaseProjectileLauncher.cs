using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Projectile.Model;
using PleasantlyGames.RPG.Runtime.Core.Projectile.View;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Contract;
using PleasantlyGames.RPG.Runtime.Core.Skill.View;
using PleasantlyGames.RPG.Runtime.Core.Skill.View.Data;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Model
{
    public abstract class BaseProjectileLauncher : IProjectileLauncher
    {
        private readonly ProjectileFactory _projectileFactory;
        private readonly SkillViewFactory _skillViewFactory;
        private readonly LocationFactory _locationFactory;
        private readonly SkillViewData _viewConfig;
        
        protected readonly List<BaseProjectile> Projectiles = new();
        protected readonly Dictionary<BaseProjectile, SkillView> ProjectileDictionary = new();
        
        public event Action<BaseProjectile, Collider> ProjectileTriggerEnter;
        public event Action<BaseProjectile> ProjectileComplete;

        protected BaseProjectileLauncher(ProjectileFactory projectileFactory, SkillViewFactory skillFactory, LocationFactory locationFactory, SkillViewData viewConfig)
        {
            _projectileFactory = projectileFactory;
            _skillViewFactory = skillFactory;
            _locationFactory = locationFactory;
            _viewConfig = viewConfig;
        }

        public abstract void Launch(float speed, Vector3 startPosition, Vector3 targetPosition);

        public abstract void Launch(float speed, Vector3 startPosition, Transform target);

        void IProjectileLauncher.ReleaseAll(bool instantDespawn = false)
        {
            for (var i = Projectiles.Count - 1; i >= 0; i--) 
                ReleaseProjectile(Projectiles[i], instantDespawn);
        }
        
        protected virtual BaseProjectile CreateProjectile(Vector3 startPosition, Vector3 direction)
        {
            var projectile = _projectileFactory.CreateProjectile();
            projectile.SetPosition(startPosition);
            projectile.transform.forward = direction;

            if (_viewConfig != null)
            {
                var view = _skillViewFactory.GetView(_viewConfig.Key, startPosition, _viewConfig.GetSize());
                projectile.SetVisual(view.gameObject, _viewConfig.Key);
                ProjectileDictionary.Add(projectile, view);
            }
            
            Projectiles.Add(projectile);
            projectile.TriggerEnter += InvokeProjectileTriggerEnter;
            projectile.Complete += InvokeProjectileComplete;

            return projectile;
        }

        public virtual void ReleaseProjectile(BaseProjectile projectile, bool instantDespawn = false)
        {
            Projectiles.Remove(projectile);
            projectile.TriggerEnter -= InvokeProjectileTriggerEnter;
            projectile.Complete -= InvokeProjectileComplete;
            
            if (ProjectileDictionary.TryGetValue(projectile, out var skillView))
            {
                skillView.transform.SetParent(null);
                _locationFactory.Location.AppendChild(skillView.transform);
                if (instantDespawn)
                    skillView.ForceDespawn();
                else
                    skillView.SmoothDespawn();
                ProjectileDictionary.Remove(projectile);
            }
            projectile.ClearVisual();
            _projectileFactory.ReleaseProjectile(projectile);
        }

        public virtual void FixedTick() { }

        protected void InvokeProjectileTriggerEnter(BaseProjectile projectile, Collider collider) => 
            ProjectileTriggerEnter?.Invoke(projectile, collider);

        protected void InvokeProjectileComplete(BaseProjectile projectile) => 
            ProjectileComplete?.Invoke(projectile);
    }
}