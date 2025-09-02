using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards;
using PleasantlyGames.RPG.Runtime.Core.Products.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Model
{
    public class PurchaseWindowPresenter : IInitializable, IDisposable
    {
        [Inject] private ProductService _productService;
        [Inject] private IWindowService _windowService;
        
        private readonly List<ProductReward> _rewards = new();

        void IInitializable.Initialize() => 
            _productService.OnPurchaseInHandle += ProductPurchaseInHandle;

        void IDisposable.Dispose() => 
            _productService.OnPurchaseInHandle -= ProductPurchaseInHandle;

        private async void ProductPurchaseInHandle(Product product)
        {
            if (_windowService.IsOpen<ProductPurchaseWindow>()) return;
            if (product.Rewards == null || product.Rewards.List.Count == 0) return;
            
            _rewards.Clear();
            _rewards.AddRange(product.Rewards.List);
            if (product.Rewards.HasBonus) 
                _rewards.AddRange(product.Rewards.BonusList); 
            
            var window = await _windowService.OpenAsync<ProductPurchaseWindow>();
            window.Setup(_rewards);
        }
    }
}