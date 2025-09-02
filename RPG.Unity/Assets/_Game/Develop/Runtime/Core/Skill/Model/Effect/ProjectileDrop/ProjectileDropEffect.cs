using System;
using System.Collections.Generic;
using System.Globalization;
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
using PleasantlyGames.RPG.Runtime.Core.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Skill.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Factory;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.ProjectileDrop
{
    public class ProjectileDropEffect : BaseSkillEffect
    {
        [Inject] private SkillViewFactory _skillViewFactory;
        [Inject] private LocationFactory _locationFactory;
        [Inject] private ProjectileFactory _projectileFactory;
        [Inject] private PopupTextFactory _popupTextFactory;
        [Inject] private UnitFactory _unitFactory;
        
        private readonly ProjectileDropData _data;
        private Vector3 _spawnOffset;
        private const float DespawnDelay = 2f;
        
        private readonly Collider[] _colliders = new Collider[10];
        private IProjectileLauncher _projectileLauncher;
        
        private int _remainingAttacks;
        private Tween _delayTween;
        
        public ProjectileDropEffect(SkillRow config, string skillNameId, ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType> customValueSheet)
            : base(config, skillNameId, customValueSheet)
        {
            _data = JsonConvertLog.DeserializeObject<ProjectileDropData>(config.EffectDataJSON);
        }
        
        public override List<(SkillValueType valueType, Func<string> variableGetter)> GetDescriptions()
        {
            var descriptions = new List<(SkillValueType valueType, Func<string> variableGetter)>
            {
                (SkillValueType.Damage, () => StringExtension.Instance.CutDouble(GetFloat(SkillValueType.Damage)).ToString(CultureInfo.InvariantCulture)),
                (SkillValueType.Amount, () => GetMaxInt(SkillValueType.Amount).ToString(CultureInfo.InvariantCulture))
            };
            return descriptions;
        }

        public override void Initialize(Func<int> levelSource)
        {
            base.Initialize(levelSource);
            var projectileConfig = Config.Views.Count > 0 ? Config.Views[0] : null;
            _projectileLauncher = new DefaultProjectileLauncher(_projectileFactory, _skillViewFactory, _locationFactory, projectileConfig);
            _projectileLauncher.ProjectileComplete += OnProjectileComplete;

            _spawnOffset = new Vector3(_data.OffsetX, _data.OffsetY, _data.OffsetZ);
        }

        public override void Execute()
        {
            base.Execute();
            
            _remainingAttacks = GetMaxInt(SkillValueType.Amount);
            Attack();
        }

        public override void Stop()
        {
            base.Stop();
            _delayTween.Stop();

            _projectileLauncher.ReleaseAll(true);

            // foreach (var view in _explosionViews) 
            //     view.ForceDespawn();
        }
        
        private void Attack()
        {
            _remainingAttacks--;

            Vector3 targetPosition = Vector3.zero;
            switch (_data.Target)
            {
                case SkillTargetType.Center:
                    targetPosition = SkillTargetProvider.GetCenterXZPosition();
                    break;
                case SkillTargetType.RandomPosition:
                    targetPosition = SkillTargetProvider.GetRandomPosition();
                    break;
            }
            var spawnPosition = targetPosition + _spawnOffset;
            _projectileLauncher.Launch(_data.Speed, spawnPosition, targetPosition);
            
            if (_remainingAttacks > 0) 
                _delayTween = Tween.Delay(_data.Delay, Attack);
        }
        
        private void OnProjectileComplete(BaseProjectile projectile)
        {
            SpawnDamageArea(projectile.transform.position);
            _projectileLauncher.ReleaseProjectile(projectile);
        }

        private void SpawnDamageArea(Vector3 position)
        {
            var viewConfig = Config.Views[1];
            var view = _skillViewFactory.GetView(viewConfig.Key, position + Vector3.up * _data.SecondViewOffsetY, viewConfig.GetSize());
            _locationFactory.Location.AppendChild(view.transform);
            view.SmoothDespawn(DespawnDelay);
            // _explosionViews.Add(view);
            //todo create despawn
            
            TakeDamage(position);
        }

        private void TakeDamage(Vector3 position)
        {
            var damage = GetPlayerDamage() * (GetFloat(SkillValueType.Damage) / 100);
            if (_data.Radius > 0)
            {
                if (Physics.OverlapSphereNonAlloc(position, _data.Radius, _colliders) > 0)
                {
                    foreach (var collider in _colliders)
                    {
                        if (collider != null && collider.TryGetComponent(out UnitView unit)) 
                            DamageUnit(unit, damage);
                    }
                }   
                for (var i = 0; i < _colliders.Length; i++) 
                    _colliders[i] = null;
            }
            else
            {
                var units = new List<UnitView>(_unitFactory.Units);
                foreach (var unit in units) 
                    DamageUnit(unit, damage);
            }
        }

        private void DamageUnit(UnitView unitView, BigDouble.Runtime.BigDouble damage)
        {
            if(!_data.TeamTypes.Contains(unitView.TeamType)) return;
            
            unitView.TakeDamage(damage);
            _popupTextFactory.SpawnFromWorldPosition(unitView.HealthBarPoint.position,
                $"-{StringExtension.Instance.CutBigDouble(damage)}", Color.white);
        }

        protected override float GetDuration() =>
            GetMaxInt(SkillValueType.Amount) * _data.Delay + _spawnOffset.magnitude / _data.Speed;
    }
}