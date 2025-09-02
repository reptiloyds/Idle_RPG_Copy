using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Projectile.Model;
using PleasantlyGames.RPG.Runtime.Core.Projectile.View;
using PleasantlyGames.RPG.Runtime.Core.Skill.View.Data;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Model
{
    public class ParabolaProjectileLauncher : BaseProjectileLauncher
    {
        private readonly float _height;

        private class ParabolaProjectile
        {
            private Vector3 _startPosition;
            private float _time;
            private float _progression;
            private Transform _target;
            private Vector3 _targetPosition;

            private Vector3 _lastTargetPosition;

            private const float DefaultHeight = 3f;
            private float _height;

            public bool IsComplete => _progression >= 1;
            public BaseProjectile Projectile { get; private set; }

            public void Setup(BaseProjectile projectile, float time, float height, Transform target)
            {
                _height = height <= 0 ? DefaultHeight : height;
                Projectile = projectile;
                _startPosition = Projectile.transform.position;
                _time = time;
                _target = target;
                _progression = 0;
            }

            public void Setup(BaseProjectile projectile, float time, float height, Vector3 position)
            {
                _height = height <= 0 ? DefaultHeight : height;
                Projectile = projectile;
                _startPosition = Projectile.transform.position;
                _time = time;
                _targetPosition = position;
                _progression = 0;
            }

            public void Clear()
            {
                Projectile = null;
                _target = null;
            }

            public void Move(float deltaTime)
            {
                _progression += deltaTime / _time;
                Vector3 newPosition;
                if (_target != null)
                {
                    if (_target.gameObject.activeSelf)
                    {
                        newPosition = VectorExtension.Parabola(_startPosition, _target.position, _height, _progression);
                        _lastTargetPosition = _target.position;   
                    }
                    else
                    {
                        _targetPosition = _lastTargetPosition;
                        _target = null;
                        newPosition = VectorExtension.Parabola(_startPosition, _lastTargetPosition, _height, _progression);
                    }
                }
                else
                    newPosition = VectorExtension.Parabola(_startPosition, _targetPosition, _height, _progression);
                
                var direction = newPosition - Projectile.transform.position;
                
                Projectile.Forward(direction);
                Projectile.MovePosition(newPosition);
            }
        }

        private readonly ObjectPool<ParabolaProjectile> _pool;
        private readonly List<ParabolaProjectile> _parabolaProjectiles = new();
        
        public ParabolaProjectileLauncher(ProjectileFactory projectileFactory, SkillViewFactory skillFactory, LocationFactory locationFactory, SkillViewData viewConfig, float height) : base(projectileFactory, skillFactory, locationFactory, viewConfig)
        {
            _height = height;
            _pool = new ObjectPool<ParabolaProjectile>(CreateFunc, GetFunc, ReleaseFunc);
        }

        public override void Launch(float speed, Vector3 startPosition, Vector3 targetPosition)
        {
            var parabolaProjectile = _pool.Get();
            var projectile = CreateProjectile(startPosition, targetPosition - startPosition);
            projectile.transform.position = startPosition; // TODO FIX SET POSITION
            projectile.Setup(speed, 100); // TODO REWORK
            var moveTime = VectorExtension.ApproximateParabolaLength(startPosition, targetPosition, _height) / speed;
            parabolaProjectile.Setup(projectile, moveTime, _height, targetPosition);
        }

        public override void Launch(float speed, Vector3 startPosition, Transform target)
        {
            var parabolaProjectile = _pool.Get();
            var projectile = CreateProjectile(startPosition, target.position - startPosition);
            projectile.Setup(speed, 100);
            var moveTime = VectorExtension.ApproximateParabolaLength(startPosition, target.position, _height) / speed;
            parabolaProjectile.Setup(projectile, moveTime, _height, target);
        }

        public override void FixedTick()
        {
            base.FixedTick();

            var deltaTime = Time.deltaTime;

            for (var i = _parabolaProjectiles.Count - 1; i >= 0; i--)
            {
                var parabola = _parabolaProjectiles[i];
                parabola.Move(deltaTime);
                if (parabola.IsComplete)
                    InvokeProjectileComplete(parabola.Projectile);
            }
        }

        private static ParabolaProjectile CreateFunc() => 
            new();

        private void GetFunc(ParabolaProjectile obj) => 
            _parabolaProjectiles.Add(obj);

        public override void ReleaseProjectile(BaseProjectile projectile, bool instantDespawn = false)
        {
            base.ReleaseProjectile(projectile, instantDespawn);

            foreach (var parabolaProjectile in _parabolaProjectiles)
            {
                if (parabolaProjectile.Projectile != projectile) continue;
                _pool.Release(parabolaProjectile);
                break;
            }
        }

        private void ReleaseFunc(ParabolaProjectile obj)
        {
            obj.Clear();
            _parabolaProjectiles.Remove(obj);
        }
    }
}