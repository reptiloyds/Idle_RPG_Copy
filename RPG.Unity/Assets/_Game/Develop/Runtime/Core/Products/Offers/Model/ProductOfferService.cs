using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Offers.Sheet;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Offers.Model
{
    public class ProductOfferService
    {
        [Inject] private BalanceContainer _balance;
        [Inject] private ProductService _productService;
        [Inject] private ISpriteProvider _spriteProvider;

        private readonly List<ProductOffer> _offers = new();

        public event Action<ProductOffer> OnOfferReady; 
        public event Action<ProductOffer> OnOfferCompleted;

        public IReadOnlyList<ProductOffer> Offers => _offers;

        [Preserve]
        public ProductOfferService()
        {
            
        }

        public void Initialize() => 
            CreateOffers();

        private void CreateOffers()
        {
            var sheet = _balance.Get<ProductOffersSheet>();

            foreach (var config in sheet)
            {
                var product = _productService.GetProduct(config.ProductId);
                if(product == null) continue;
                
                var offer = new ProductOffer(product, config, _spriteProvider);
                offer.Initialize();
                offer.OnReady += OnReady;
                offer.OnCompleted += OnCompleted;
                _offers.Add(offer);
            }
        }

        private void OnReady(ProductOffer offer) => 
            OnOfferReady?.Invoke(offer);

        private void OnCompleted(ProductOffer offer) => 
            OnOfferCompleted?.Invoke(offer);
    }
}