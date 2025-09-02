using System.Collections;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.Core.ShopHub.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    public class PermanentProductCollectionView : ShopElementView
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private ProductView _productPrefab;
        [SerializeField] private MergedProductView _mergedProductPrefab;

        private readonly List<ProductView> _views = new(16);
        private readonly Dictionary<string, MergedProductView> _mergedViews = new(16);

        [Inject] private IObjectResolver _resolver;
        [Inject] private ProductService _service;
        [Inject] private IWindowService _windowService;

        public IReadOnlyDictionary<string, MergedProductView> MergedViews => _mergedViews;
        public IReadOnlyList<ProductView> Views => _views;

        private void Awake() =>
            Initialize();

        private void Initialize()
        {
            foreach (var kvp in _service.MergedProducts) 
                CreateMergedViewFor(kvp.Value);
            foreach (var product in _service.Products)
            {
                if (product.Placement != ProductPlacement.Permanent) continue;
                if (product.IsMerged) continue;
                CreateViewFor(product);
            }
        }
        

        private void CreateViewFor(Product product)
        {
            var view = _resolver.Instantiate(_productPrefab, _container);
            _views.Add(view);
            view.Setup(product);
        }

        private void CreateMergedViewFor(IReadOnlyList<Product> products)
        {
            if(products.Count == 0) return;
            var mergedKey = products[0].MergedKey;
            var mergedView = _resolver.Instantiate(_mergedProductPrefab, _container);
            mergedView.SetupMerged(products);
            _mergedViews.Add(mergedKey, mergedView);
        }
    }
}