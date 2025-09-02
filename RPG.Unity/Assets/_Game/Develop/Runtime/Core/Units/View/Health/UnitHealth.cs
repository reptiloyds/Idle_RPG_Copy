using System;
using Sirenix.OdinInspector;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Health
{
    [Serializable]
    public abstract class UnitHealth
    {
        public event Action OnZeroHealth;
        public event Action<BigDouble.Runtime.BigDouble> OnHeal;
        public event Action<BigDouble.Runtime.BigDouble> OnDamage;
        public event Action OnChangeMaxValue;
        
        protected HealthBarFactory HealthBarFactory;

        private bool _hideWhenFull;
        
        [ShowInInspector] public BigDouble.Runtime.BigDouble MaxValue { get; protected set; }
        [ShowInInspector] public BigDouble.Runtime.BigDouble Value { get; protected set; }

        public double ProgressValue => (Value / MaxValue).ToDouble();
        public HealthBar Bar { get; protected set; }
        public bool HideWhenFull => _hideWhenFull;
        public event Action OnHideFlagChanged;

        protected UnitHealth(HealthBarFactory healthBarFactory) => 
            HealthBarFactory = healthBarFactory;

        public virtual void Initialize() { }
        
        public virtual void Dispose() { }

        public void SetHideFlag(bool hideWhenFull)
        {
            _hideWhenFull = hideWhenFull;
            OnHideFlagChanged?.Invoke();
        }

        public virtual void ApplyDamage(BigDouble.Runtime.BigDouble value)
        {
            if(Value == 0) return;
            
            Value -= value;
            OnDamage?.Invoke(value);
            
            //if (Value <= 0)
                //Value = -0;
            if (Value <= 0)
                OnZeroHealth?.Invoke();
        }

        public virtual void ApplyHeal(BigDouble.Runtime.BigDouble value)
        {
            if(Value == 0) return;
            if (Value >= MaxValue) return;
            
            Value += value;
            OnHeal?.Invoke(value);
            
            if (Value > MaxValue) 
                Value = MaxValue;
        }

        public void FullHealth() => 
            ApplyHeal(MaxValue);

        protected void SetNewMaxValue(BigDouble.Runtime.BigDouble newMaxValue)
        {
            if (BigDouble.Runtime.BigDouble.Abs(Value - MaxValue) < 0.01f)
            {
                MaxValue = newMaxValue;
                Value = MaxValue;
            }
            else
                MaxValue = newMaxValue;
            
            OnChangeMaxValue?.Invoke();
        }

        protected void ResetHealth() => 
            Value = MaxValue;
    }
}