using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    public class MergedProductView : HorizontalProductView
    {
        private IReadOnlyList<Product> _products;

        public void SetupMerged(IReadOnlyList<Product> products)
        {
            _products = products;
            foreach (var product in _products)
            {
                if (product.Access.LimitIsOver) continue;
                if (!product.Access.IsUnlocked) 
                    product.Access.OnUnlocked += OnProductUnlocked;

                if (!product.Rewards.Character.IsUnlocked)
                    product.Rewards.Character.OnUnlocked += OnRewardUnlock;
            }
            
            SelectAvailableProduct();
        }

        public override void Setup(Product product)
        {
            if (!_products.Contains(product)) return;
            base.Setup(product);
        }

        public void Select(Product product)
        {
            if (product.Access.LimitIsOver) return;
            if (!product.Access.IsUnlocked) return;
            if (!product.Rewards.Character.IsUnlocked) return;
            Setup(product);
        }

        private void SelectAvailableProduct()
        {
            ClearProduct();
            foreach (var product in _products)
            {
                if (product.Access.LimitIsOver) continue;
                if (!product.Access.IsUnlocked) continue;
                if (!product.Rewards.Character.IsUnlocked) continue;
                Setup(product);
                break;
            }
            gameObject.SetActive(Product != null);
        }

        private void OnProductUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnProductUnlocked;
            SelectAvailableProduct();
        }

        private void OnRewardUnlock(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnRewardUnlock;
            SelectAvailableProduct();
        }

        protected override void OnLimitIsOver()
        {
            base.OnLimitIsOver();
            SelectAvailableProduct();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (var product in _products)
            {
                if (!product.Access.IsUnlocked)
                    product.Access.OnUnlocked -= OnProductUnlocked;
                if (!product.Rewards.Character.IsUnlocked)
                    product.Rewards.Character.OnUnlocked -= OnRewardUnlock;
            }
        }
    }
}