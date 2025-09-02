using System;
using R3;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents
{
    public abstract class UITimer : MonoBehaviour
    {
        private readonly SerialDisposable _serialDisposable = new();
        private bool _isVisible;
        
        protected float StartValue;
        protected ReadOnlyReactiveProperty<float> TimerProperty;
        
        public bool IsActive { get; private set; }
        public event Action OnComplete;

        protected virtual void OnEnable()
        {
            _isVisible = true;
            if(IsActive)
                UpdateValue(TimerProperty.CurrentValue);
        }

        protected void OnDisable() => 
            _isVisible = false;

        public virtual void Listen(ReadOnlyReactiveProperty<float> property, bool toEnd = true)
        {
            //TODO TEST
            if(IsActive)
                Stop();
            //
            
            IsActive = true;
            TimerProperty = property;
            StartValue = TimerProperty.CurrentValue;
            _serialDisposable.Disposable = TimerProperty
                .ObserveOnCurrentSynchronizationContext()
                .Subscribe(value =>
                {
                    if(_isVisible)
                        UpdateValue(value);
                    if (toEnd && value <= 0f)
                        Complete();
                });
        }

        public abstract void UpdateValue(float leftSeconds);

        public virtual void Stop()
        {
            IsActive = false;
            _serialDisposable.Disposable?.Dispose();
        }

        protected virtual void Complete()
        {
            Stop();
            OnComplete?.Invoke();
        }
    }
}