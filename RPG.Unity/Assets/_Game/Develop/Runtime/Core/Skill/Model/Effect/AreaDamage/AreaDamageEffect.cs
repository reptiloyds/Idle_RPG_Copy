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
using PleasantlyGames.RPG.Runtime.Core.Units.Factory;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.AreaDamage
{
    public class AreaDamageEffect : BaseSkillEffect
    {
        [Inject] private UnitFactory _unitFactory;
        [Inject] private SkillViewFactory _skillViewFactory;
        [Inject] private LocationFactory _locationFactory;
        [Inject] private PopupTextFactory _popupTextFactory;

        private int _remainingAttacks;
        private readonly AreaDamageData _data;
        private Tween _delayTween;
        private readonly Collider[] _colliders = new Collider[10];

        private const float DESPAWN_DELAY = 1F;
        
        public AreaDamageEffect(SkillRow config, string skillNameId, ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType> customValueSheet)
            : base(config, skillNameId, customValueSheet)
        {
            _data = JsonConvertLog.DeserializeObject<AreaDamageData>(config.EffectDataJSON);
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

            SkillTargetRequest = new SkillTargetRequest()
            {
                TeamFilter = _data.TeamTypes,
                SearchTargetType = SkillSearchTargetType.InArea,
            };
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
        }

        private void Attack()
        {
            _remainingAttacks--;
            var response = SkillTargetProvider.GetTarget(SkillTargetRequest, ValidUnits);
            if (response.Success)
            {
                SpawnDamageArea(ValidUnits.GetRandomElement().transform.position);
                ValidUnits.Clear();
            }
            else
                SpawnDamageArea(response.AlternativePosition);
            
            if (_remainingAttacks > 0) 
                _delayTween = Tween.Delay(_data.Delay, Attack);
        }

        private void SpawnDamageArea(Vector3 position)
        {
            var viewConfig = Config.Views[0];
            var view = _skillViewFactory.GetView(viewConfig.Key, position, viewConfig.GetSize());
            _locationFactory.Location.AppendChild(view.transform);
            view.SmoothDespawn(DESPAWN_DELAY);
            
            TakeDamage(position);
        }

        private void TakeDamage(Vector3 position)
        {
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
            GetMaxInt(SkillValueType.Amount) * _data.Delay;
    }
}