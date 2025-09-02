using System;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Save;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Model
{
    public class ProductAccess : IUnlockable, IDisposable
    {
        private readonly ProductRow _config;
        private readonly ProductData _data;
        
        private readonly AllUnlockable _unlockable;

        public bool HasLimit => _config.PurchaseLimit > 0;
        public bool LimitIsOver => HasLimit && _data.LimitCounter >= _config.PurchaseLimit;
        public bool IsUnlocked => _unlockable == null || _unlockable.IsUnlocked;
        public string Condition => _unlockable == null ? string.Empty : _unlockable.Condition;
        public Product Product { get; private set; }
        
        public event Action<IUnlockable> OnUnlocked;
        public event Action OnLimitIsOver;

        public ProductAccess(ProductRow config, ProductData data, AllUnlockable unlockable, Product product)
        {
            _config = config;
            _data = data;
            _unlockable = unlockable;
            Product = product;
        }

        public void Initialize()
        {
            if (_unlockable is { IsUnlocked: false }) 
                _unlockable.OnUnlocked += OnUnlock;
        }

        public void AddUnlockable(IUnlockable unlockable) => 
            _unlockable.Add(unlockable);

        public void IncrementPurchasesAmount()
        {
            _data.Purchases++;
            _data.LimitCounter++;
            if (LimitIsOver)
                OnLimitIsOver?.Invoke();
        }

        private void OnUnlock(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlock;
            OnUnlocked?.Invoke(this);
        }

        public void Dispose()
        {
            if (_unlockable is { IsUnlocked: false }) 
                _unlockable.OnUnlocked -= OnUnlock;
        }
    }
}