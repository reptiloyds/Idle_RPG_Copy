using System;
using System.Collections.Generic;
using System.Globalization;
using PleasantlyGames.RPG.Runtime.Core.Ally;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Type;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.PopupNumbers.Model;
using PleasantlyGames.RPG.Runtime.Core.Projectile.Model;
using PleasantlyGames.RPG.Runtime.Core.Projectile.View;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Contract;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Type;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.View;
using PleasantlyGames.RPG.Runtime.Core.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Skill.View;
using PleasantlyGames.RPG.Runtime.Core.Skill.View.Data;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Bomber
{
    public class BomberEffect : BaseSkillEffect
    {
        [Inject] private ProjectileFactory _projectileFactory;
        [Inject] private SkillViewFactory _skillFactory;
        [Inject] private LocationFactory _locationFactory;
        [Inject] private AllySquad _allySquad;
        [Inject] private LocationUnitCollection _locationUnits;
        [Inject] private PopupTextFactory _popupTextFactory;
        
        private readonly BomberData _data;
        private const float FakeDuration = 35f;
        private float _attackDelay;
        private bool _isAttackOnCooldown;
        private UnitView _target;
        private Vector3 _offset;
        private int _remainingBombs;
        
        private SkillView _bomberView;
        private ProjectileLauncherView _launcherView;
        
        private IProjectileLauncher _projectileLauncher;
        private const float RotationSpeed = 5f;
        private const float DespawnDelay = 2F;
        private int _awaitProjectiles;

        private readonly Collider[] _colliders = new Collider[10];

        public BomberEffect(SkillRow config, string skillNameId, ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType> customValueSheet)
            : base(config, skillNameId, customValueSheet)
        {
            _data = JsonConvertLog.DeserializeObject<BomberData>(config.EffectDataJSON);
        }
        
        public override List<(SkillValueType valueType, Func<string> variableGetter)> GetDescriptions()
        {
            var descriptions = new List<(SkillValueType valueType, Func<string> variableGetter)>
            {
                (SkillValueType.Damage, () => StringExtension.Instance.CutDouble(GetFloat(SkillValueType.Damage)).ToString(CultureInfo.InvariantCulture)),
                (SkillValueType.Amount, () => StringExtension.Instance.CutDouble(GetMaxInt(SkillValueType.Amount)).ToString(CultureInfo.InvariantCulture)),
            };
            return descriptions;
        }

        public override void Initialize(Func<int> levelSource)
        {
            base.Initialize(levelSource);

            _offset = new Vector3(0, _data.Height, 0);
            var viewConfig = Config.Views.Count > 0 ? Config.Views[0] : null;
            
            switch (_data.LaunchType)
            {
                case LaunchType.Default:
                    _projectileLauncher = new DefaultProjectileLauncher(_projectileFactory, _skillFactory, _locationFactory, viewConfig);
                    break;
                case LaunchType.Parabola:
                    _projectileLauncher = new ParabolaProjectileLauncher(_projectileFactory, _skillFactory, _locationFactory, viewConfig, _data.ParabolaHeight);
                    break;
            }
            
            _projectileLauncher.ProjectileComplete += OnProjectileComplete;
            
            SkillTargetRequest = new SkillTargetRequest
            {
                TeamFilter = _data.TeamTypes,
                SearchTargetType = SkillSearchTargetType.InArea,
            };
        }
        
        public override void Execute()
        {
            base.Execute();

            _remainingBombs = GetMaxInt(SkillValueType.Amount);
            ResetAttackDelay();
            SpawnBomber(GetSpawnPosition());
            UpdateTarget();
        }
        
        public override void Stop()
        {
            CleanUp();
            base.Stop();
        }
        
        private void CleanUp()
        {
            if(IsCompleted) return;
            
            _launcherView = null;
            _isAttackOnCooldown = false;
            _attackDelay = 0;
            _remainingBombs = 0;
            _awaitProjectiles = 0;
            
            _locationUnits.OnUnitEnter -= OnUnitEnter;
            if (_bomberView != null)
            {
                _bomberView.SmoothDespawn();
                _bomberView = null;
            }
            
            if(_target != null)
                ClearTarget();
            
            _projectileLauncher.ReleaseAll(true);
        }

        public override void Tick()
        {
            base.Tick();

            if(IsCompleted) return;
            
            if (_isAttackOnCooldown)
            {
                _attackDelay -= Time.deltaTime;
                if (_attackDelay <= 0)
                    _isAttackOnCooldown = false;
            }
            
            if(_target == null) return;

            var bomberPos = _bomberView.transform.position;
            var targetPos = _target.transform.position + _offset;
            bool canAttack = Vector3.Distance(targetPos, _bomberView.transform.position) <= _data.AttackRadius;
            var direction = targetPos - bomberPos;
            if (canAttack)
            {
                if (!_isAttackOnCooldown && _remainingBombs > 0)
                    Attack();
            }
            else
                _bomberView.transform.position += direction.normalized * _data.MovementSpeed * Time.deltaTime;

            var singleStep = Time.deltaTime * RotationSpeed;
            Vector3 newDirection = Vector3.RotateTowards(_bomberView.transform.forward, direction, singleStep, 0.0f);
            _bomberView.transform.rotation = Quaternion.LookRotation(newDirection);
        }
        
        public override void FixedTick()
        {
            base.FixedTick();
            _projectileLauncher.FixedTick();
        }

        private void Attack()
        {
            _projectileLauncher.Launch(_data.Speed, _launcherView.GetShootPoint().position, _target.transform.position);
            _isAttackOnCooldown = true;
            ResetAttackDelay();
            
            _remainingBombs--;
            _awaitProjectiles++;
        }

        private void ResetAttackDelay() => 
            _attackDelay = 1 / _data.AttackSpeed;

        private Vector3 GetSpawnPosition()
        {
            var units = _allySquad.GetUnits();
            var spawnPosition = Vector3.zero;
            if (units.Count > 0)
            {
                foreach (var unit in units) 
                    spawnPosition += unit.transform.position;
                spawnPosition /= units.Count;   
            }

            spawnPosition += _offset;
            return spawnPosition;
        }
        
        private void SpawnBomber(Vector3 spawnPosition)
        {
            var viewConfig = Config.Views.Count > 1 ? Config.Views[1] : null;
            if (viewConfig == null)
            {
                Debug.LogError($"{typeof(SkillViewData)} is null");
                return;
            }
            
            var sourceView = _skillFactory.GetView(viewConfig.Key, spawnPosition, viewConfig.GetSize());
            _bomberView = sourceView;

            if (!_bomberView.TryGetComponent(out ProjectileLauncherView launcherView))
            {
                Debug.LogError($"Can`t find {typeof(ProjectileLauncherView)}");
                return;
            }
            
            _launcherView = launcherView;
        }

        private void OnUnitEnter(UnitView unitView)
        {
            if (!_data.TeamTypes.Contains(unitView.TeamType)) return;
            _locationUnits.OnUnitEnter -= OnUnitEnter;
            UpdateTarget();
        }

        private void UpdateTarget()
        {
            _target = GetTarget();
            if (_target == null)
            {
                _locationUnits.OnUnitEnter += OnUnitEnter;
                return;
            }
            
            _target.OnDie += OnTargetDisposed;
        }

        private void OnTargetDisposed(UnitView unitView)
        {
            ClearTarget();
            UpdateTarget();
        }

        private void ClearTarget()
        {
            _target.OnDie -= OnTargetDisposed;
            _target = null;
        }

        private UnitView GetTarget()
        {
            UnitView target;
            switch (_data.TargetType)
            {
                case TargetType.Single:
                    if(_target != null && !_target.IsDead)
                        target = _target;
                    else
                        GetRandomValidUnit(out target);
                    break;
                case TargetType.Multiple:
                    GetRandomValidUnit(out target);
                    break;
                default:
                    target = null;
                    break;
            }
            
            SkillTargetResponse GetRandomValidUnit(out UnitView resultUnit)
            {
                var response = SkillTargetProvider.GetTarget(SkillTargetRequest, ValidUnits);
                resultUnit = response.Success ? ValidUnits.GetRandomElement() : null;
                ValidUnits.Clear();
                return response;
            }

            return target;
        }

        private void OnProjectileComplete(BaseProjectile projectile)
        {
            var viewConfig = Config.Views[2];
            var explosionView = _skillFactory.GetView(viewConfig.Key, projectile.transform.position, viewConfig.GetSize());
            _locationFactory.Location.AppendChild(explosionView.transform);
            explosionView.SmoothDespawn(DespawnDelay);
            
            if (Physics.OverlapSphereNonAlloc(projectile.transform.position, _data.ExplosionRadius, _colliders) > 0)
            {
                foreach (var collider in _colliders)
                {
                    if (collider != null && collider.TryGetComponent(out UnitView unit)) 
                        DamageUnit(unit);
                }
            }

            for (var i = 0; i < _colliders.Length; i++) 
                _colliders[i] = null;
            
            _projectileLauncher.ReleaseProjectile(projectile);

            _awaitProjectiles--;
            if (_remainingBombs == 0 && _awaitProjectiles == 0)
            {
                CleanUp();
                Complete();
            } 
        }
        
        private void DamageUnit(UnitView unitView)
        {
            if(!_data.TeamTypes.Contains(unitView.TeamType)) return;

            var damage = GetPlayerDamage() * (GetFloat(SkillValueType.Damage) / 100);
            unitView.TakeDamage(damage);
            _popupTextFactory.SpawnFromWorldPosition(unitView.HealthBarPoint.position,
                $"-{StringExtension.Instance.CutBigDouble(damage)}", Color.white);
        }

        protected override float GetDuration() => 
            FakeDuration;
    }
}