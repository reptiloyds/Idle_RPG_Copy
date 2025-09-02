using System;
using System.Collections.Generic;
using System.Globalization;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Type;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Skill.View;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.StatModifier
{
    public sealed class StatModifierEffect : BaseSkillEffect
    {
        [Inject] private SkillViewFactory _skillViewFactory;
        
        private readonly StatModifierData _data;
        private readonly List<UnitView> _affectedUnits = new();
        private Tween _durationTween;
        
        private readonly Dictionary<UnitView, SkillView> _viewDictionary = new();
        
        private const float _radiusDelta = 0.35f;

        public StatModifierEffect(SkillRow config, string skillNameId, ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType> customValueSheet)
            : base(config, skillNameId, customValueSheet)
        {
            _data = JsonConvertLog.DeserializeObject<StatModifierData>(config.EffectDataJSON);
        }

        public override List<(SkillValueType valueType, Func<string> variableGetter)> GetDescriptions()
        {
            var descriptions = new List<(SkillValueType valueType, Func<string> variableGetter)>
            {
                (SkillValueType.Duration, () => StringExtension.Instance.CutDouble(GetFloat(SkillValueType.Duration)).ToString(CultureInfo.InvariantCulture)),
                (SkillValueType.Value, () => StringExtension.Instance.CutDouble(GetFloat(SkillValueType.Value)).ToString(CultureInfo.InvariantCulture))
            };
            return descriptions;
        }
        
        public override void Initialize(Func<int> levelSource)
        {
            base.Initialize(levelSource);

            SkillTargetRequest = new SkillTargetRequest()
            {
                TeamFilter = _data.TeamTypes,
                IncludeTypeFilter = _data.IncludeTypes,
                ExcludeTypeFilter = _data.ExcludeTypes,
                SearchTargetType = SkillSearchTargetType.All,
            };
        }

        public override void Execute()
        {
            base.Execute();
            var response = SkillTargetProvider.GetTarget(SkillTargetRequest, ValidUnits);
            if (response.Success)
            {
                foreach (var unit in ValidUnits) 
                    AppendUnit(unit);
                ValidUnits.Clear();
                _durationTween = Tween.Delay(GetDuration(), CancelEffect);
            }
        }

        public override void Stop()
        {
            base.Stop();
            _durationTween.Stop();
            for (var i = _affectedUnits.Count - 1; i >= 0; i--) 
                RemoveUnit(_affectedUnits[i], true);
        }

        private void CancelEffect()
        {
            for (var i = _affectedUnits.Count - 1; i >= 0; i--) 
                RemoveUnit(_affectedUnits[i], false);
        }

        private void AppendUnit(UnitView unitView)
        {
            _affectedUnits.Add(unitView);
            unitView.DisposeEvent += OnUnitDispose;
            var stat = unitView.GetStat(_data.StatType);
            var modifier = new Stats.Model.StatModifier(GetFloat(SkillValueType.Value), _data.ModType, (int)_data.ModType, this, true);
            stat.AddModifier(modifier);
            
            var viewConfig = Config.Views.Count > 0 ? Config.Views[0] : null;
            if (viewConfig != null)
            {
                var position = unitView.Visual.transform.position;
                position.y = 0;
                var view = _skillViewFactory.GetView(viewConfig.Key, position, Vector3.one * (unitView.Radius + _radiusDelta));
                view.Follow(unitView.Visual.transform, Vector3.down * unitView.Visual.transform.position.y);
                _viewDictionary.Add(unitView, view);
            }
        }

        private void OnUnitDispose(UnitView unitView)
        {
            RemoveUnit(unitView, true);
        }

        private void RemoveUnit(UnitView unitView, bool instantDespawn)
        {
            if (_viewDictionary.TryGetValue(unitView, out var view))
            {
                view.StopFollow();
                if (instantDespawn)
                    view.ForceDespawn();
                else
                    view.SmoothDespawn();
                _viewDictionary.Remove(unitView);
            } 
            
            unitView.DisposeEvent -= OnUnitDispose;
            _affectedUnits.Remove(unitView);
            var stat = unitView.GetStat(_data.StatType);
            stat.RemoveAllModifiersFromSource(this);
        }

        protected override float GetDuration() => 
            GetFloat(SkillValueType.Duration);
    }
}