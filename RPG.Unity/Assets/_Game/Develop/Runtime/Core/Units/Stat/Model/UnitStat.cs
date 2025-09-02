using System;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Config;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Save;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model
{
    [Serializable]
    public class UnitStat : BaseStat
    {
        private UnitStatConfig _config;
        protected StatData StatData;
        private BaseValueFormula _valueFormula;
        
        [ShowInInspector, PropertyOrder(-3)] public UnitStatType Type => StatData.Type;
        [ShowInInspector, PropertyOrder(-2)] private BigDouble.Runtime.BigDouble _finalValue; // Only for inspector display
        [ShowInInspector, PropertyOrder(-1)] public int Level => StatData.Level;
        
        private bool _isStableDirty = true;
        private BigDouble.Runtime.BigDouble _stableValue;
		
        public BigDouble.Runtime.BigDouble StableValue
        {
            get {
                if(_isStableDirty || lastBaseValue != BaseValue) {
                    lastBaseValue = BaseValue;
                    _stableValue = CalculateValue(true);
                    _isStableDirty = false;
                }
                return _stableValue;
            }
        }

        public string TypeString { get; private set; }
        public bool IsUnlocked => StatData.IsUnlocked;
        public bool IsUpgradable => _config.MaxLevel > 1;
        public bool IsLevelMax => StatData.Level == _config.MaxLevel;
        public Sprite Sprite { get; }
        
        public event Action OnLevelUp;

        public UnitStat(StatData statData, UnitStatConfig config, Sprite sprite, BaseValueFormula valueFormula = null)
        {
            TypeString = config.StatType.ToString();
            StatData = statData;
            _config = config;
            Sprite = sprite;

            _valueFormula = valueFormula ?? _config.ValueFormulaType.CreateFormula(_config.ValueFormulaJSON);

            RecalculateBaseValue();
        }

        protected override void OnRefModifiersChanged()
        {
            _isStableDirty = true;
            base.OnRefModifiersChanged();
        }

        [Button]
        public void LevelUp()
        {
            StatData.Level++;
            RecalculateBaseValue();
            OnValueChanges();
            OnLevelUp?.Invoke();
        }

        public void SetLevel(int level)
        {
            if(level < 0) return;
            
            StatData.Level = level;
            RecalculateBaseValue();
            OnValueChanges();
        }

        public void Unlock()
        {
            if (StatData.IsUnlocked) return;

            StatData.IsUnlocked = true;
            if(Level == 0)
                LevelUp();
        }

        public void SetValueFormula(BaseValueFormula valueFormula)
        {
            _valueFormula = valueFormula;
            RecalculateBaseValue();
        }

        public override void AddModifier(StatModifier mod)
        {
            base.AddModifier(mod);
            if (!mod.Temporary) 
                _isStableDirty = true;
            _finalValue = Value;
        }

        public override bool RemoveModifier(StatModifier mod)
        {
            var result = base.RemoveModifier(mod);
            if (result && !mod.Temporary) 
                _isStableDirty = true;
            _finalValue = Value;
            return result;
        }

        public override bool RemoveAllModifiersFromSource(object source)
        {
            var result = base.RemoveAllModifiersFromSource(source);
            if(result)
                _isStableDirty = true;
            return result;
        }

        private void RecalculateBaseValue()
        {
            BaseValue = _valueFormula.CalculateBigDouble(StatData.Level);
            _finalValue = Value;
            _isStableDirty = true;
        }

#if RPG_PROD
        [Conditional("DUMMY_UNUSED_DEFINE")]
#endif
        public void SetValue_CheatOnly(int value) => 
            _value = value;
    }
}