using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Projectile.Model;
using PleasantlyGames.RPG.Runtime.Core.Projectile.Type;
using PleasantlyGames.RPG.Runtime.Core.Projectile.View;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.Weapons
{
    public class RangeWeapon : Weapon
    {
        public struct Impact
        {
            public GameObject Effect;
            public float ReleaseTime;
        }

        [SerializeField] private Transform _shootPoint;
        [FormerlySerializedAs("_shootSfx")] [SerializeField] private SFX_Shot shotSfx;
        [SerializeField] private Vector3 _muzzleOffset;
        [SerializeField] private ParticleSystem _muzzle;
        [SerializeField] private string _projectileKey;
        [SerializeField] private string _impactKey;
        [SerializeField, MinValue(0), HideIf("@this._impactKey == \"\" || this._impactKey == null")] private float _impactLifeTime = 0.5f;
        [SerializeField, MinValue(0), HideIf("@this._impactKey == \"\" || this._impactKey == null")] private Vector3 _impactSize = Vector3.one;
        [SerializeField] private ProjectileBehaviourType _projectileBehaviour;
        [SerializeField, MinValue(0)] private float _projectileSpeed;
        [SerializeField, MinValue(0)] private float _projectileMaxDistance;
        private const float _distanceToFakeShot = 0.75f;
        
        [Inject] private ProjectileFactory _factory;
        [Inject] private IAudioService _audioService;
        [Inject] private LocationFactory _locationFactory;

        private readonly List<Impact> _impacts = new();
        
        private readonly List<BaseProjectile> _projectiles = new();
        private bool _hasImpact;
        
        public override WeaponType Type => WeaponType.Range;

        private void Awake()
        {
            _shootPoint ??= transform;
            _hasImpact = !string.IsNullOrEmpty(_impactKey);
        }

        public void SetShootPoint(Transform shootPoint) => 
            _shootPoint = shootPoint;

        public override void PerformAttack(UnitView target) => 
            Shoot(target);

        public override void CancelAttack() => 
            ClearTargets();

        public void Shoot(UnitView targetUnitView)
        {
            if (_projectileBehaviour == ProjectileBehaviourType.OneHit && IsFlatDistanceToTargetLessOrEqualFakeShot(targetUnitView.transform))
                HandleHit(targetUnitView, targetUnitView.ProjectileTarget.position);
            else
                SpawnProjectile(targetUnitView);

            if (_muzzle != null)
            {
                _muzzle.transform.position = _shootPoint.position;
                _muzzle.transform.localPosition += _muzzleOffset;
                _muzzle.transform.forward = _shootPoint.forward;
                _muzzle.Play();  
            }
            
            _audioService
                .CreateLocalSound(shotSfx)
                .WithPosition(_shootPoint.position)
                .Play();
        }

        private void SpawnProjectile(UnitView targetUnitView)
        {
            var projectile = _factory.CreateProjectile(_projectileKey);
            projectile.SetPosition(_shootPoint.position);
            projectile.Setup(_projectileSpeed, _projectileMaxDistance);
            
            if (_shootPoint.position.y > targetUnitView.transform.position.y + targetUnitView.Height)
                projectile.MoveTo(targetUnitView.ProjectileTarget, Vector3.zero);
            else
                projectile.MoveTo(targetUnitView.transform, new Vector3(0, _shootPoint.position.y, 0));
            
            projectile.TriggerEnter += OnProjectileTriggerEnter;
            projectile.Complete += OnProjectileComplete;
            
            _projectiles.Add(projectile);   
        }

        private bool IsFlatDistanceToTargetLessOrEqualFakeShot(Transform target)
        {
            return Vector2.Distance(new Vector2(_shootPoint.position.x, _shootPoint.position.z),
                new Vector2(target.position.x, target.position.z)) <= _distanceToFakeShot;
        }

        private void OnProjectileTriggerEnter(BaseProjectile projectile, Collider other)
        {
            if(!other.TryGetComponent(out UnitView unit)) return;
            if(unit.TeamType == TeamType) return;
            
            switch (_projectileBehaviour)
            {
                case ProjectileBehaviourType.OneHit:
                    ReleaseProjectile(projectile);
                    break;
                case ProjectileBehaviourType.MultiHit:
                    projectile.MoveByDirection(projectile.TotalDirection * projectile.LeftDistance);
                    break;
            }

            HandleHit(unit, projectile.transform.position);
        }

        private void HandleHit(UnitView target, Vector3 position)
        {
            if (_hasImpact)
            {
                var impactView = _factory.CreateParticle(_impactKey);
                _locationFactory.Location.AppendChild(impactView.transform);
                impactView.transform.position = position;
                impactView.transform.localScale = _impactSize;
                _impacts.Add(new Impact
                {
                    Effect = impactView,
                    ReleaseTime = Time.time + _impactLifeTime
                });
            }
            AttackPerformedTo(target);
        }

        private void OnProjectileComplete(BaseProjectile projectile)
        {
            if(projectile.LeftDistance <= 0)
                ReleaseProjectile(projectile);
            else
                projectile.MoveByDirection(projectile.TotalDirection * projectile.LeftDistance);
        }

        private void ReleaseProjectile(BaseProjectile projectile)
        {
            projectile.TriggerEnter -= OnProjectileTriggerEnter;
            projectile.Complete -= OnProjectileComplete;
            _projectiles.Remove(projectile);
            _factory.ReleaseProjectile(projectile);
        }

        public void Dispose()
        {
            for (int i = _projectiles.Count - 1; i >= 0; i--) 
                ReleaseProjectile(_projectiles[i]);
            _projectiles.Clear();
            ClearAllImpacts();
        }

        private void Update()
        {
            for (var i = _impacts.Count - 1; i >= 0; i--)
            {
                if (Time.time < _impacts[i].ReleaseTime) continue;
                _factory.ReleaseParticle(_impactKey, _impacts[i].Effect);
                _impacts.RemoveAt(i);
            }
        }

        private void ClearAllImpacts()
        {
            for (var i = _impacts.Count - 1; i >= 0; i--)
            {
                _factory.ReleaseParticle(_impactKey, _impacts[i].Effect);
                _impacts.RemoveAt(i);
            }
        }

        private void ClearTargets()
        {
            foreach (var projectile in _projectiles) 
                projectile.MoveByDirection(projectile.TotalDirection * projectile.LeftDistance);
        }
    }
}
