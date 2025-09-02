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
using PrimeTween;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.ShootingObject
{
    public class ShootingObjectEffect : BaseSkillEffect
    {
        [Inject] private ProjectileFactory _projectileFactory;
        [Inject] private SkillViewFactory _skillFactory;
        [Inject] private LocationFactory _locationFactory;
        [Inject] private AllySquad _allySquad;
        [Inject] private PopupTextFactory _popupTextFactory;
        
        private readonly ShootingObjectData _data;

        private readonly Vector3 _spawnOffset = new(0, 4f, 0);
        private const float DespawnDelay = 2f;
        private Vector3 _spawnPosition;
        private SkillTargetRequest _targetRequest;

        private IProjectileLauncher _projectileLauncher;
        private Tween _attackDelayTween;
        private UnitView _target;

        private SkillView _sourceView;
        private ProjectileLauncherView _launcherView;
        private Tween _clearTween;
        private readonly Collider[] _colliders = new Collider[10];
        private float _attackDelay;
        
        public ShootingObjectEffect(SkillRow config, string skillNameId, ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType> customValueSheet)
            : base(config, skillNameId, customValueSheet)
        {
            _data = JsonConvertLog.DeserializeObject<ShootingObjectData>(config.EffectDataJSON);
        }
        
        public override List<(SkillValueType valueType, Func<string> variableGetter)> GetDescriptions()
        {
            var descriptions = new List<(SkillValueType valueType, Func<string> variableGetter)>
            {
                (SkillValueType.Damage, () => StringExtension.Instance.CutDouble(GetFloat(SkillValueType.Damage)).ToString(CultureInfo.InvariantCulture)),
                (SkillValueType.AttackSpeed, () => StringExtension.Instance.CutDouble(GetFloat(SkillValueType.AttackSpeed)).ToString(CultureInfo.InvariantCulture)),
                (SkillValueType.Duration, () => StringExtension.Instance.CutDouble(GetFloat(SkillValueType.Duration)).ToString(CultureInfo.InvariantCulture))
            };
            return descriptions;
        }

        public override void Initialize(Func<int> levelSource)
        {
            base.Initialize(levelSource);
            
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
            
            _projectileLauncher.ProjectileTriggerEnter += OnProjectileTriggerEnter;
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

            _attackDelay = 1 / GetFloat(SkillValueType.Duration);
            
            var units = _allySquad.GetUnits();
            _spawnPosition = Vector3.zero;
            if (units.Count > 0)
            {
                foreach (var unit in units) 
                    _spawnPosition += unit.transform.position;
                _spawnPosition /= units.Count;   
            }
            _spawnPosition += _spawnOffset;

            // if (_data.TargetType == TargetType.Single) TODO CHECK IT
            //     GetTarget(out _singleTarget);

            SpawnSource();
            Attack();
            
            _clearTween = Tween.Delay(GetDuration(), CancelEffect);
        }

        public override void Stop()
        {
            base.Stop();
            _attackDelayTween.Stop();
            _clearTween.Stop();
            _launcherView = null;

            if (_sourceView != null)
            {
                _sourceView.ForceDespawn();
                _sourceView = null;
            }
            
            _projectileLauncher.ReleaseAll(true);
        }

        public override void FixedTick()
        {
            base.FixedTick();
            _projectileLauncher.FixedTick();
        }

        private void SpawnSource()
        {
            var viewConfig = Config.Views.Count > 1 ? Config.Views[1] : null;
            if (viewConfig == null)
            {
                Debug.LogError($"{typeof(SkillViewData)} is null");
                return;
            }
            
            var sourceView = _skillFactory.GetView(viewConfig.Key, _spawnPosition, viewConfig.GetSize());
            _sourceView = sourceView;

            if (!_sourceView.TryGetComponent(out ProjectileLauncherView launcherView))
            {
                Debug.LogError($"Can`t find {typeof(ProjectileLauncherView)}");
                return;
            }
            
            _launcherView = launcherView;
        }

        private void CancelEffect()
        {
            if (_sourceView != null)
            {
                _sourceView.SmoothDespawn();
                _sourceView = null;   
            }
            _attackDelayTween.Stop();
            _launcherView = null;
        }

        private void Attack()
        {
            _target = GetTarget();
            if (_target != null)
            {
                _projectileLauncher.Launch(_data.Speed, _launcherView.GetShootPoint().position, _target.ProjectileTarget);
                _launcherView.Focus(_target.ProjectileTarget.position);
                _launcherView.PlayAnimation();
            }
            
            _attackDelayTween = Tween.Delay(_attackDelay, Attack);
        }

        private void OnProjectileComplete(BaseProjectile projectile)
        {
            if (projectile.LeftDistance <= 0)
            {
                SpawnDamageEffect(projectile.transform.position);
                if (_data.Radius > 0) 
                    TakeDamageInRadius(projectile.transform.position);
                _projectileLauncher.ReleaseProjectile(projectile);
            }
            else
                projectile.MoveByDirection(projectile.TotalDirection * projectile.LeftDistance);
        }

        private void OnProjectileTriggerEnter(BaseProjectile projectile, Collider collider)
        {
            if (!collider.TryGetComponent(out UnitView unit)) return;

            SpawnDamageEffect(projectile.transform.position);
            if (_data.Radius <= 0) 
                TakeDamage(unit);
            else
                TakeDamageInRadius(unit.ProjectileTarget.position);
            _projectileLauncher.ReleaseProjectile(projectile);
        }

        private void SpawnDamageEffect(Vector3 position)
        {
            var viewConfig = Config.Views.Count > 2 ? Config.Views[2] : null;
            if (viewConfig != null)
            {
                var view = _skillFactory.GetView(viewConfig.Key, position, viewConfig.GetSize());
                _locationFactory.Location.AppendChild(view.transform);
                view.SmoothDespawn(DespawnDelay);
                //todo create despawn
            }
        }
        
        private void TakeDamageInRadius(Vector3 position)
        {
            if (Physics.OverlapSphereNonAlloc(position, _data.Radius, _colliders) > 0)
            {
                foreach (var collider in _colliders)
                {
                    if (collider != null && collider.TryGetComponent(out UnitView unit)) 
                        TakeDamage(unit);
                }
            }

            for (var i = 0; i < _colliders.Length; i++) 
                _colliders[i] = null;
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

        private void TakeDamage(UnitView unitView)
        {
            if(!_data.TeamTypes.Contains(unitView.TeamType)) return;
            
            var damage = GetPlayerDamage() * (GetFloat(SkillValueType.Damage) / 100);
            unitView.TakeDamage(damage);
            _popupTextFactory.SpawnFromWorldPosition(unitView.HealthBarPoint.position,
                $"-{StringExtension.Instance.CutBigDouble(damage)}", Color.white);
        }

        protected override float GetDuration() => 
            GetFloat(SkillValueType.Duration);
    }
}