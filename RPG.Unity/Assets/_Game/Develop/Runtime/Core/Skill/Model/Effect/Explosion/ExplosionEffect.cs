using System;
using System.Collections.Generic;
using System.Globalization;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Type;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.PopupNumbers.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Skill.View;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Explosion
{
    public class ExplosionEffect : BaseSkillEffect
    {
        [Inject] private SkillViewFactory _skillViewFactory;
        [Inject] private LocationFactory _locationFactory;
        [Inject] private PopupTextFactory _popupTextFactory;
        
        private readonly ExplosionData _data;
        
        private Tween _explosionTween;
        private readonly List<UnitView> _exclusions = new();
        private readonly List<SkillView> _explosionObjects = new();
        private readonly List<SkillView> _explosionEffects = new();
        private readonly Collider[] _colliders = new Collider[10];
        
        private const float DespawnDelay = 2F;
        
        public ExplosionEffect(SkillRow config, string skillNameId, ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType> customValueSheet)
            : base(config, skillNameId, customValueSheet)
        {
            _data = JsonConvertLog.DeserializeObject<ExplosionData>(config.EffectDataJSON);
        }

        public override List<(SkillValueType valueType, Func<string> variableGetter)> GetDescriptions()
        {
            var descriptions = new List<(SkillValueType valueType, Func<string> variableGetter)>
            {
                (SkillValueType.Damage, () => StringExtension.Instance.CutDouble(GetFloat(SkillValueType.Damage)).ToString(CultureInfo.InvariantCulture)),
                (SkillValueType.Amount, () => _data.Amount.ToString(CultureInfo.InvariantCulture)),
            };
            return descriptions;
        }

        public override void Initialize(Func<int> levelSource)
        {
            base.Initialize(levelSource);

            SkillTargetRequest = new SkillTargetRequest()
            {
                TeamFilter = _data.TeamTypes,
                SearchTargetType = SkillSearchTargetType.InArea,
            };
        }

        public override void Execute()
        {
            base.Execute();

            for (var i = 0; i < _data.Amount; i++)
            {
                var response = SkillTargetProvider.GetTarget(SkillTargetRequest, ValidUnits, _exclusions);
                if (response.Success)
                {
                    var unit = ValidUnits.GetRandomElement();
                    _exclusions.Add(unit);
                    SpawnExplosiveObject(unit.transform.position);   
                }
                else
                    SpawnExplosiveObject(response.AlternativePosition);
                ValidUnits.Clear();   
            }
            _exclusions.Clear();
        }

        public override void Stop()
        {
            base.Stop();

            foreach (var explosionObject in _explosionObjects) 
                explosionObject.ForceDespawn();
            _explosionObjects.Clear();

            foreach (var explosionEffect in _explosionEffects) 
                explosionEffect.ForceDespawn();
            _explosionEffects.Clear();
            
            _explosionTween.Stop();
        }

        private void SpawnExplosiveObject(Vector3 position)
        {
            var viewConfig = Config.Views[0];
            var explosionObject = _skillViewFactory.GetView(viewConfig.Key, position, viewConfig.GetSize());
            _locationFactory.Location.AppendChild(explosionObject.transform);

            _explosionObjects.Add(explosionObject);
            _explosionTween = Tween.Delay(_data.Delay, () => TakeDamage(position, explosionObject));
        }

        private void TakeDamage(Vector3 position, SkillView explosionObject)
        {
            explosionObject?.ForceDespawn();
            
            var viewConfig = Config.Views[1];
            var explosionEffect = _skillViewFactory.GetView(viewConfig.Key, position, viewConfig.GetSize());
            _locationFactory.Location.AppendChild(explosionEffect.transform);
            explosionEffect.SmoothDespawn(DespawnDelay);
            _explosionEffects.Add(explosionEffect);
            
            if (Physics.OverlapSphereNonAlloc(position, _data.Radius, _colliders) > 0)
            {
                foreach (var collider in _colliders)
                {
                    if (collider != null && collider.TryGetComponent(out UnitView unit)) 
                        DamageUnit(unit);
                }
            }

            for (var i = 0; i < _colliders.Length; i++) 
                _colliders[i] = null;
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
            _data.Delay;
    }
}