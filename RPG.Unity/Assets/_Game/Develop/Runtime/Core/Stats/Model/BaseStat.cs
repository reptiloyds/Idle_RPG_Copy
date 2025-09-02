using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using Sirenix.OdinInspector;

namespace PleasantlyGames.RPG.Runtime.Core.Stats.Model
{
    [Serializable]
    public class BaseStat
    {
        public BigDouble.Runtime.BigDouble BaseValue;

        protected bool isDirty = true;
        protected BigDouble.Runtime.BigDouble lastBaseValue;

        protected BigDouble.Runtime.BigDouble _value;

        public BigDouble.Runtime.BigDouble Value
        {
            get
            {
                if (isDirty || lastBaseValue != BaseValue)
                {
                    lastBaseValue = BaseValue;
                    _value = CalculateValue(false);
                    isDirty = false;
                }

                return _value;
            }
        }

        private BaseStat _modifierRef;
        [ShowInInspector]
        protected readonly List<StatModifier> statModifiers = new();
        public IReadOnlyList<StatModifier> StatModifiers => statModifiers;
        public event Action OnModifiersChanged;
        public event Action OnValueChanged;

        public BaseStat()
        {
        }

        public BaseStat(float baseValue) : this()
        {
            BaseValue = baseValue;
        }

        public void SetModifierReference(BaseStat stat)
        {
            _modifierRef = stat;
            _modifierRef.OnModifiersChanged += OnRefModifiersChanged; // TODO unsubscribe
        }

        protected virtual void OnRefModifiersChanged()
        {
            isDirty = true;
            OnValueChanges();
        }

        public virtual void AddModifier(StatModifier mod)
        {
            isDirty = true;
            statModifiers.Add(mod);
            OnModifiersChanged?.Invoke();
            OnValueChanges();
        }

        public virtual bool RemoveModifier(StatModifier mod)
        {
            var result = statModifiers.Remove(mod);
            if (result)
            {
                isDirty = true;
                OnModifiersChanged?.Invoke();
                OnValueChanges();
            }

            return result;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            var numRemovals = 0;
            for (var i = 0; i < statModifiers.Count; i++)
            {
                var index = i - numRemovals;
                if (statModifiers[index].Source != source) continue;
                numRemovals++;
                statModifiers.RemoveAt(index);
            }

            var result = numRemovals > 0;
            if (result)
            {
                isDirty = true;
                OnModifiersChanged?.Invoke();
                OnValueChanges();
            }

            return result;
        }

        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0; //if (a.Order == b.Order)
        }

        private void SortModifiers()
        {
            statModifiers.Sort(CompareModifierOrder);
        }

        protected BigDouble.Runtime.BigDouble CalculateValue(bool skipTemporaryMods)
        {
            BigDouble.Runtime.BigDouble finalValue = BaseValue;
            BigDouble.Runtime.BigDouble sumPercentAdd = 0;
            BigDouble.Runtime.BigDouble groupKAdditive = 1;
            BigDouble.Runtime.BigDouble sumGroupKAdditive = 1;
            BigDouble.Runtime.BigDouble sumKAdditive = 1;

            SortModifiers();

            for (var i = 0; i < statModifiers.Count; i++)
            {
                var mod = statModifiers[i];
                if (skipTemporaryMods && mod.Temporary) continue;
                ApplyMod(mod, i, statModifiers);
            }

            if (_modifierRef != null)
            {
                _modifierRef.SortModifiers();
                for (var i = 0; i < _modifierRef.StatModifiers.Count; i++)
                {
                    var mod = _modifierRef.StatModifiers[i];
                    if (skipTemporaryMods && mod.Temporary) continue;
                    ApplyMod(mod, i, _modifierRef.StatModifiers);
                }
            }

            void ApplyMod(StatModifier mod, int index, IReadOnlyList<StatModifier> modifiers)
            {
                switch (mod.Type)
                {
                    case StatModType.Flat:
                        finalValue += mod.Value;
                        break;
                    case StatModType.GroupKAdditive:
                        groupKAdditive += mod.Value;
                        var nextMod = modifiers.Next(index);
                        if (modifiers.IsLast(index))
                        {
                            sumGroupKAdditive *= groupKAdditive;
                            finalValue *= sumGroupKAdditive;
                            sumGroupKAdditive = 1;
                            groupKAdditive = 1;
                        }
                        else if (mod.Type == nextMod.Type && mod.Order != nextMod.Order)
                        {
                            sumGroupKAdditive *= groupKAdditive;
                            groupKAdditive = 1;
                        }
                        else if (mod.Type != nextMod.Type)
                        {
                            sumGroupKAdditive *= groupKAdditive;
                            groupKAdditive = 1;
                            if (nextMod.Type != StatModType.KAdditive)
                            {
                                finalValue *= sumGroupKAdditive;
                                sumGroupKAdditive = 1;
                            }
                        }

                        break;
                    case StatModType.KAdditive:
                        sumKAdditive += mod.Value;
                        if (modifiers.IsLast(index) || modifiers.Next(index).Type != StatModType.KAdditive)
                        {
                            if (sumGroupKAdditive > 1 && sumKAdditive > 1)
                                finalValue *= sumGroupKAdditive + sumKAdditive;
                            else
                            {
                                if (sumGroupKAdditive > 1)
                                    finalValue *= sumGroupKAdditive;
                                else if (sumKAdditive > 1)
                                    finalValue *= sumKAdditive;
                            }

                            sumKAdditive = 0;
                            sumGroupKAdditive = 1;
                        }

                        break;
                    case StatModType.PercentAdd:
                        sumPercentAdd += mod.Value;
                        if (modifiers.IsLast(index) || modifiers.Next(index).Type != StatModType.PercentAdd)
                        {
                            finalValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }
                        break;
                    case StatModType.PercentMult:
                        finalValue *= 1 + mod.Value;
                        break;
                }
            }


            return finalValue;
        }

        protected void OnValueChanges() => 
            OnValueChanged?.Invoke();
    }
}