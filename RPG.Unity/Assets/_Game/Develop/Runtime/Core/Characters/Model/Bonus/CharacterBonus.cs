using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus.Conditions;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus
{
    public abstract class CharacterBonus<T> : ICharacterBonus where T : struct, Enum 
    {
        [Inject] private ITranslator _translator;
        
        protected readonly CharacterRow.Elem Config;
        protected T EffectType;
        private StatModifier _modifier;
        private readonly BaseValueFormula _valueFormula;
        private readonly IBonusCondition _condition;
        private int _bonusLevel = -1;
        private bool _isBonusApplied;

        protected Character Character;
        protected readonly List<BaseStat> Stats = new();

        public int Level => _bonusLevel;
        public BonusConditionType ConditionType => _condition.Type;
        public bool IsUnlocked => _bonusLevel > 0;

        public Sprite Sprite { get; }
        public event Action<ICharacterBonus> OnLevelUp;

        protected CharacterBonus(CharacterRow.Elem config, IBonusCondition condition, Sprite sprite)
        {
            Config = config;
            EffectType = Config.GetEffectType<T>();
            Sprite = sprite;
            _valueFormula = Config.EffectFormulaType.CreateFormula(Config.EffectFormulaJSON);
            _condition = condition;
        }

        public bool IsAlwaysApply()
        {
            switch (ConditionType)
            {
                default:
                case BonusConditionType.Level:
                    return true;
                case BonusConditionType.Evolution:
                    return false;
            }
        }

        public virtual void Initialize(Character character)
        {
            Character = character;
            FillTargetStats();
            
            _bonusLevel = GetMetConditions();
            UpdateModifier();
            
            switch (ConditionType)
            {
                case BonusConditionType.Level:
                    character.OnLevelUp += OnCharacterLevelUp;
                    break;
                case BonusConditionType.Evolution:
                    character.OnEvolve += OnCharacterEvolve;
                    break;
            }
        }

        protected abstract void FillTargetStats();

        public int GetMetConditions() => _condition.GetMetAmount(Character);
        public string GetEnhanceCondition() => _condition.GetEnhanceCondition(Character);
        public string GetUnlockCondition() => _condition.GetUnlockCondition(Character);
        public string GetStatName() => _translator.Translate(EffectType.ToString());
        public string GetStatValue(int level) => _modifier.Type.GetFormattedModifier(GetValue(level));
        private void OnCharacterLevelUp(Character character) => TryUpdateModifier();
        private void OnCharacterEvolve(Character character) => TryUpdateModifier();

        private void TryUpdateModifier()
        {
            var metConditions = GetMetConditions();
            if (metConditions <= _bonusLevel) return;
            _bonusLevel = metConditions;
            UpdateModifier();
            OnLevelUp?.Invoke(this);
        }

        private void UpdateModifier()
        {
            var oldModifier = _modifier;
            _modifier = new(GetValue(_bonusLevel), Config.EffectModType, this, GroupOrder.CharacterBonus);

            if (IsAlwaysApply() || Character.IsEquipped)
            {
                if (!_isBonusApplied)
                {
                    _isBonusApplied = true;
                    foreach (var stat in Stats)
                        stat.AddModifier(_modifier);
                }
                else
                {
                    foreach (var stat in Stats)
                        stat.ReplaceModifier(oldModifier, _modifier);   
                }
            }
        }

        public virtual void OnTakeOff()
        {
            if (IsAlwaysApply()) return;
            if(!_isBonusApplied) return;
            foreach (var stat in Stats)
                stat.RemoveModifier(_modifier);
        }

        public virtual void OnEquip()
        {
            if (IsAlwaysApply()) return;
            if(_isBonusApplied) return;
            foreach (var stat in Stats)
                stat.AddModifier(_modifier);
        }

        private BigDouble.Runtime.BigDouble GetValue(int level) => _valueFormula.CalculateBigDouble(level);
    }
}