using System;
using PleasantlyGames.RPG.Runtime.Core.Resource.Save;
using PleasantlyGames.RPG.Runtime.Core.Resource.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using UnityEngine;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.Model
{
    public class ResourceModel
    {
        private readonly ResourceSheet.Row _config;
        private readonly ResourceData _data;
        
        public ResourceType Type => _config.Id;
        public BigDouble.Runtime.BigDouble Value => _data.Value;
        public Sprite Sprite { get; }
        public bool InvisibleWhenZero => _config.InvisibleWhenZero;

        public event Action OnChange;
        public event Action<ResourceModel, BigDouble.Runtime.BigDouble> OnAdd;
        public event Action<ResourceModel, BigDouble.Runtime.BigDouble> OnSpend;

        public ResourceModel(ResourceSheet.Row config, ResourceData data, Sprite sprite)
        {
            _config = config;
            _data = data;
            Sprite = sprite;
        }

        public bool IsEnough(BigDouble.Runtime.BigDouble value) => 
            Value >= value;

        public void Set(BigDouble.Runtime.BigDouble value)
        {
            if(value < 0) return;
            _data.Value = value;
            OnChange?.Invoke();
        }

        public void Add(BigDouble.Runtime.BigDouble value)
        {
            if (value == 0)
            {
                Logger.LogWarning($"Attempt to spend zero value to {typeof(ResourceModel)} type of {Type}");
                return;
            }
            if (value < 0)
            {
                Logger.LogError($"Attempt to spend a negative value to {typeof(ResourceModel)} type of {Type}");
                return;
            }
            
            var newValue = _data.Value + value;
            _data.Value = newValue;
            
            OnChange?.Invoke();
            OnAdd?.Invoke(this, value);
        }

        public void Spend(BigDouble.Runtime.BigDouble value)
        {
            if (value == 0)
            {
                Logger.LogWarning($"Attempt to spend zero value to {typeof(ResourceModel)} type of {Type}");
                return;
            }
            if (value < 0)
            {
                Logger.LogError($"Attempt to spend a negative value to {typeof(ResourceModel)} type of {Type}");
                return;
            }

            if (value > Value)
            {
                Logger.LogError($"Trying to spend more in {typeof(ResourceModel)} type of {Type}");
                return;
            }
            
            var newValue = _data.Value - value;
            _data.Value = BigDouble.Runtime.BigDouble.Ceiling(newValue);
            _data.Value = newValue;
            
            OnChange?.Invoke();
            OnSpend?.Invoke(this, value);
        }
    }
}