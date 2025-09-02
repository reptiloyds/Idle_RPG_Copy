using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Type;
using PleasantlyGames.RPG.Runtime.Core.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using Sirenix.OdinInspector;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Base
{
    public abstract class BaseSkillEffect
    {
        [Inject] protected SkillTargetProvider SkillTargetProvider;
        [Inject] protected UnitStatService StatService;
        
        protected SkillTargetRequest SkillTargetRequest;
        protected readonly SkillRow Config;
        protected readonly Dictionary<SkillValueType, BaseValueFormula> FormulaDictionary = new();
        protected Func<int> LevelSource { get; private set; }
        
        protected readonly List<UnitView> ValidUnits = new();
        
        [ShowInInspector, LabelText("Skill name")]
        private readonly string _skillNameId;

        private readonly ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType> _customValueSheet;

        private UnitStat _playerDamage;

        private bool _isActive;
        private bool _isCompleted;
        
        public bool IsActive => _isActive;
        public bool IsCompleted => _isCompleted;

        public event Action OnComplete;
        
        protected BaseSkillEffect(SkillRow config, string skillNameId, ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType> customValueSheet)
        {
            Config = config;
            _skillNameId = skillNameId;
            _customValueSheet = customValueSheet;
        }

        public virtual void Initialize(Func<int> levelSource)
        {
            _playerDamage = StatService.GetPlayerStat(UnitStatType.Damage);
            LevelSource = levelSource;
            foreach (var elem in Config) 
                CreateFormula(elem);
        }

        public virtual void Execute()
        {
            _isActive = true;
            _isCompleted = false;
        }

        public virtual void Stop()
        {
            _isActive = false;
            _isCompleted = true;
        }

        public float GetDelay() =>
            Config.Delay;

        public float GetTotalDuration() =>
            GetDelay() + GetDuration();

        public abstract List<(SkillValueType valueType, Func<string> variableGetter)> GetDescriptions();
        
        protected abstract float GetDuration();

        protected virtual void Complete()
        {
            _isCompleted = true;
            OnComplete?.Invoke();
        }

        protected void CreateFormula(SkillRow.Elem config)
        {
            BaseValueFormula formula;
            if (config.ValueFormulaType == FormulaType.CustomSheet)
                formula = _customValueSheet.GetValueFormula(Config.Id, config.ValueType);
            else
                formula = config.ValueFormulaType.CreateFormula(config.ValueFormulaJSON);
            FormulaDictionary.Add(config.ValueType, formula);
        }
        
        protected float GetFloat(SkillValueType key) => 
            (float)GetDouble(key);

        private double GetDouble(SkillValueType valueType)
        {
            if (FormulaDictionary.TryGetValue(valueType, out var formula))
                return formula.CalculateBigDouble(LevelSource.Invoke()).ToDouble();

            return -1;
        }

        protected int GetMaxInt(SkillValueType key) => 
            (int)Math.Ceiling(GetDouble(key));

        protected int GetMinInt(SkillValueType key) => 
            (int)GetDouble(key);

        protected BigDouble.Runtime.BigDouble GetPlayerDamage() => _playerDamage.Value;

        public virtual void Tick() { }
        public virtual void FixedTick() {}
    }
}