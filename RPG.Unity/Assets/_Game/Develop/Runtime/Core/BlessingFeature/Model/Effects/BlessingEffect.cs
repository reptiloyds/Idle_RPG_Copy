using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using Sirenix.OdinInspector;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model.Effects
{
    public abstract class BlessingEffect
    {
        private readonly StatModType _statModType;
        private readonly BaseValueFormula _valueFormula;
        private readonly bool _zeroOnFirstLevel;
        private readonly GroupOrder _groupOrder;

        protected int Level;
        protected bool IsEnabled;

        [ShowInInspector] private string _name;
        
        public string StatName { get; private set; }
        public StatModifier StatModifier { get; private set; }

        protected BlessingEffect(StatModType statModType, BaseValueFormula valueFormula, string statName, bool zeroOnFirstLevel, GroupOrder groupOrder)
        {
            _statModType = statModType;
            _valueFormula = valueFormula;
            _zeroOnFirstLevel = zeroOnFirstLevel;
            _groupOrder = groupOrder;
            StatName = statName;
        }

        public void SetDebugName(string debugName) => 
            _name = debugName;

        public virtual void Enable() => 
            IsEnabled = true;

        public virtual void Disable() => 
            IsEnabled = false;

        public void SetLevel(int level)
        {
            Level = level;
            UpdateModifier();
        }

        protected virtual void UpdateModifier()
        {
            if (Level == 1 && _zeroOnFirstLevel)
                StatModifier = new StatModifier(0, _statModType, this, _groupOrder);
            else
                StatModifier = new StatModifier(_valueFormula.CalculateBigDouble(Level), _statModType, this, _groupOrder);
        }
    }
}