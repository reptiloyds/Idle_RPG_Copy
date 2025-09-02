using System;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Rewards
{
    public abstract class ProductReward : IUnlockable
    {
        private readonly IUnlockable _unlockable;
        private readonly ProductElem _config;
        public abstract ProductItemType Type { get; }
        
        public abstract Sprite Sprite { get; }
        public abstract string Name { get; }
        public Color BackColor { get; }
        
        public bool IsUnlocked => _unlockable == null || _unlockable.IsUnlocked;
        public string Condition => _unlockable == null ? string.Empty : _unlockable.Condition;
        public string MergeKey => _config.MergeKey;
        public bool IsBonus => _config.IsBonus;

        public event Action<IUnlockable> OnUnlocked;

        protected ProductReward(Color backColor, ProductElem config, IUnlockable unlockable = null)
        {
            BackColor = backColor;
            _unlockable = unlockable;
            _config = config;
        }

        public virtual void Initialize()
        {
            if (!IsUnlocked) 
                _unlockable.OnUnlocked += OnUnlock;
        }

        private void OnUnlock(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlock;
            OnUnlocked?.Invoke(this);
        }

        public abstract void Apply();

        public virtual object GetCertainProductType()
        {
            return null;
        }
    }
}