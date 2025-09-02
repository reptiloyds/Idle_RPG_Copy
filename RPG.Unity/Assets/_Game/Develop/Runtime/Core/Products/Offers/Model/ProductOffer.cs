using System;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Offers.Sheet;
using PleasantlyGames.RPG.Runtime.Core.UI.Hub.Sides;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Offers.Model
{
    public class ProductOffer
    {
        private readonly ProductOffersSheet.Row _config;

        public UISideType UISideType => _config.UISideType;
        public Sprite Sprite { get; }
        public Sprite ButtonSprite { get; }
        public Product Product { get; }
        public bool IsReady { get; private set; }
        public event Action<ProductOffer> OnReady;
        public event Action<ProductOffer> OnCompleted;
        
        public ProductOffer(Product product, ProductOffersSheet.Row config, ISpriteProvider spriteProvider)
        {
            Product = product;
            _config = config;
            Sprite = spriteProvider.GetSprite(_config.SpriteName);
            ButtonSprite = Product.Rewards.Character.Sprite;
        }

        public void Initialize()
        {
            CheckCondition();
            if (!Product.Access.IsUnlocked) 
                Product.Access.OnUnlocked += OnUnlock;

            if (!Product.Rewards.Character.IsUnlocked) 
                Product.Rewards.Character.OnUnlocked += OnProductUnlocked;

            if (!Product.Access.LimitIsOver) 
                Product.Access.OnLimitIsOver += OnProductLimitIsOver;
        }

        private void OnUnlock(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlock;
            CheckCondition();
        }

        private void OnProductUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnProductUnlocked;
            CheckCondition();
        }

        private void OnProductLimitIsOver()
        {
            Product.Access.OnLimitIsOver -= OnProductLimitIsOver;
            OnCompleted?.Invoke(this);
        }

        private void CheckCondition()
        {
            if(IsReady) return;
            IsReady = Product.Access.IsUnlocked && !Product.Access.LimitIsOver && Product.Rewards.Character.IsUnlocked;
            if (IsReady) 
                OnReady?.Invoke(this);
        }
    }
}