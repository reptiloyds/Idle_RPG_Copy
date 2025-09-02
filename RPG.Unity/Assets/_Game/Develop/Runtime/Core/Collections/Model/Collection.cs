using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Collections.Save;
using PleasantlyGames.RPG.Runtime.Core.Collections.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Collections.Model
{
    public class Collection : IDisposable
    {
        private readonly ITranslator _translator;
        private readonly CollectionRow _config;
        private readonly CollectionData _data;

        private readonly List<Item> _items;
        private readonly UnitStat _stat;
        private readonly BaseValueFormula _enhanceFormula;
        private string _formattedName;
        private BigDouble.Runtime.BigDouble _bonusValue;
        private string _formattedBonus;
        private string _formattedBonusDelta;
        private bool _canEnhance;
        private bool _isUnlocked;
        private StatModifier _statModifier;
        private int _enhanceLevel;
        private readonly string _modifierSourceName;

        public ItemType Type => _config.Type;
        public int Level => _data.Level;
        public bool IsLevelMax => _data.Level == _config.MaxLevel;
        public bool CanEnhance => _canEnhance;
        public bool IsUnlocked => _isUnlocked;
        public string FormattedName => _formattedName;
        public string FormattedBonus => _formattedBonus;
        public string FormattedBonusDelta => _formattedBonusDelta;
        public IReadOnlyList<Item> Items => _items;

        public event Action<Collection> OnUpdate;

        public Collection(List<Item> items, UnitStat stat, ITranslator translator,
            CollectionRow config, CollectionData data, BaseValueFormula formula)
        {
            _items = items;
            _stat = stat;
            _translator = translator;
            _config = config;
            _data = data;
            _modifierSourceName = $"Collection {_config.Id}";

            _enhanceFormula = formula;
            UpdateEnhance();
            RecalculateValue();
            UpdateName();
            CreateModifier();

            _stat.AddModifier(_statModifier);

            foreach (var item in _items)
            {
                item.OnUnlock += OnItemUnlock;
                item.OnLevelUp += OnItemLevelUp;
            }
        }

        public void Dispose()
        {
            foreach (var item in _items)
            {
                item.OnUnlock -= OnItemUnlock;
                item.OnLevelUp -= OnItemLevelUp;
            }
        }

        private void OnItemUnlock(Item obj)
        {
            UpdateEnhance();
            RecalculateDeltaValue();
            OnUpdate?.Invoke(this);
        }

        private void OnItemLevelUp(Item item)
        {
            UpdateEnhance();
            RecalculateDeltaValue();
            OnUpdate?.Invoke(this);
        }

        public void Enhance()
        {
            _data.Level = _enhanceLevel;
            UpdateEnhance();
            RecalculateValue();
            RecalculateDeltaValue();
            UpdateName();
            UpdateOwnedModifiers();
            OnUpdate?.Invoke(this);
        }

        private void RecalculateValue()
        {
            _bonusValue = _enhanceFormula.CalculateBigDouble(_data.Level);
            var formattedValue = _config.EffectModType.GetFormattedModifier(_bonusValue);
            _formattedBonus = $"{_translator.Translate(_config.EffectType.ToString())} {formattedValue}";
        }

        private void RecalculateDeltaValue()
        {
            if (IsLevelMax || _enhanceLevel == _data.Level)
                _formattedBonusDelta = string.Empty;
            else
            {
                var nextValue = _enhanceFormula.CalculateBigDouble(_enhanceLevel);
                _formattedBonusDelta = _config.EffectModType.GetFormattedModifier(nextValue - _bonusValue);
            }
        }

        private void UpdateName() =>
            _formattedName =
                $"{_translator.Translate(_config.LocalizationToken)} {TranslationConst.LevelPrefix}. {Level}";

        private void UpdateOwnedModifiers()
        {
            var oldMod = _statModifier;
            CreateModifier();
            _stat.ReplaceModifier(oldMod, _statModifier);
        }

        private void CreateModifier() =>
            _statModifier = new StatModifier(_enhanceFormula.CalculateBigDouble(Level),
                _config.EffectModType, _modifierSourceName, GroupOrder.None);

        private void UpdateEnhance()
        {
            if (!IsLevelMax)
            {
                _isUnlocked = true;
                int minCommonLevel = int.MaxValue;
                foreach (var item in _items)
                {
                    if (!item.IsUnlocked)
                    {
                        _isUnlocked = false;
                        _enhanceLevel = 0;
                        minCommonLevel = 0;
                        break;
                    }

                    minCommonLevel = Math.Min(item.Level, minCommonLevel);
                }

                var nextLevel = Level + 1;
                _enhanceLevel = Math.Max(minCommonLevel, nextLevel);
                _enhanceLevel = Math.Min(_enhanceLevel, _config.MaxLevel);
                _canEnhance = minCommonLevel >= nextLevel;
            }
            else
            {
                _isUnlocked = true;
                _enhanceLevel = _data.Level;
                _canEnhance = false;
            }
        }
    }
}