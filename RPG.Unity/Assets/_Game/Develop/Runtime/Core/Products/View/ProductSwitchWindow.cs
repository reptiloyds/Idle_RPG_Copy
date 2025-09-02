using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    public class ProductSwitchWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private RectTransform _container;
        [SerializeField] private SwitchProductRewardView _prefab;
        
        [Inject] private ProductService _productService;
        [Inject] private MessageBuffer _messageBuffer;
        [Inject] private IObjectResolver _resolver;

        private ObjectPool<SwitchProductRewardView> _viewPool;
        private readonly List<SwitchProductRewardView> _views = new();
        private Product _selectedProduct;
        private ProductView _productView;

        protected override void Awake()
        {
            base.Awake();
            CreatePool();
        }

        private void CreatePool()
        {
            _viewPool = new ObjectPool<SwitchProductRewardView>(CreateFunc, GetAction, ReleaseAction, DestroyAction);
            return;

            SwitchProductRewardView CreateFunc() => 
                _resolver.Instantiate(_prefab, _container);

            void GetAction(SwitchProductRewardView view)
            {
                view.gameObject.SetActive(true);
                view.OnClick += OnViewClick;
            }

            void ReleaseAction(SwitchProductRewardView view)
            {
                view.gameObject.SetActive(false);
                view.OnClick -= OnViewClick;
            }

            void DestroyAction(SwitchProductRewardView view) =>
                Destroy(view.gameObject);
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        public void Setup(ProductView productView)
        {
            Clear();
            if (!_productService.MergedProducts.TryGetValue(productView.Product.MergedKey, out var mergedProducts)) return;

            var siblingIndex = 0;
            foreach (var product in mergedProducts)
            {
                var view = _viewPool.Get();
                _views.Add(view);
                view.Setup(product, product.Rewards.GetMergeReward());
                view.gameObject.SetActive(!product.Access.LimitIsOver && product.Access.IsUnlocked);
                view.transform.SetSiblingIndex(siblingIndex++);
            } 
            _productView = productView;
            
            Select(_productView.Product);
        }

        private void Clear()
        {
            foreach (var view in _views) 
                _viewPool.Release(view);
            _views.Clear();
        }

        private void OnViewClick(SwitchProductRewardView view)
        {
            if (!view.Reward.IsUnlocked)
            {
                _messageBuffer.Send(view.Reward.Condition);
                return;
            }
            
            Select(view.Product);
        }

        private void Select(Product product)
        {
            var isProductNew = _selectedProduct != product && _selectedProduct != null;
            _selectedProduct = product;
            foreach (var view in _views)
            {
                if (view.Product != product) 
                    view.Unselect();
                else
                    view.Select();
            }

            if (isProductNew) 
                _productView.Setup(_selectedProduct); 
        }
    }
}