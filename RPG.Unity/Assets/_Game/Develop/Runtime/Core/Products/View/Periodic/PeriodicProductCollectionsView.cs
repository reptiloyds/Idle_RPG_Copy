using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Model.Periodic;
using PleasantlyGames.RPG.Runtime.Core.ShopHub.View;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View.Periodic
{
    public class PeriodicProductCollectionsView : ShopElementView
    {
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _verticalContainer;
        [SerializeField] private PeriodicLabel _labelPrefab;
        [SerializeField] private PeriodicTimer _timerPrefab;
        [SerializeField] private ProductView _horizontalPrefab;
        [SerializeField] private ProductView _verticalPrefab;
        [SerializeField] private RectTransform _linePrefab;
        [SerializeField] private List<PeriodicProductTabView> _tabs;
        [SerializeField] private List<PeriodicProductTabButton> _buttons;
        [SerializeField] private PeriodicType _defaultType = PeriodicType.Monthly;

        [Inject] private ProductService _productService;
        [Inject] private IObjectResolver _resolver;
        [Inject] private IAnalyticsService _analytics;
        private PeriodicType _selectedType;
        
        private ObjectPool<ProductView> _horizontalPool;
        private ObjectPool<ProductView> _verticalPool;

        private readonly List<RectTransform> _content = new();
        private readonly Dictionary<PeriodicType, List<ProductView>> _viewDictionary = new();

        public IReadOnlyDictionary<PeriodicType, List<ProductView>> ViewDictionary => _viewDictionary;
        public IReadOnlyList<PeriodicProductTabView> Tabs => _tabs;
        public IReadOnlyList<PeriodicProductTabButton> Buttons => _buttons;

        public event Action<PeriodicType> OnNewProducts;

        private void Awake()
        {
            _scroll.onValueChanged.AddListener(CheckVisibility);
            foreach (var button in _buttons)
                button.OnClicked += OnButtonClicked;
            Initialize();
        }

        private void OnDestroy()
        {
            _scroll.onValueChanged.RemoveListener(CheckVisibility);
            foreach (var button in _buttons)
                button.OnClicked -= OnButtonClicked;
            if (_productService != null) 
                _productService.PeriodicProducts.OnRefreshed -= OnProductsRefreshed;
        }

        private void Initialize()
        {
            _horizontalPool = CreatePool(_horizontalPrefab, _root);
            _verticalPool = CreatePool(_verticalPrefab, _verticalContainer);
            _productService.PeriodicProducts.OnRefreshed += OnProductsRefreshed;

            for (var i = 0; i < _tabs.Count; i++)
            {
                if (i > 0 && i < _tabs.Count)
                {
                    var line = Instantiate(_linePrefab, _root);
                    _content.Add(line);
                }
                var tab = _tabs[i];
                var products = _productService.PeriodicProducts.GetActualProducts(tab.Type);
                if (!_viewDictionary.TryGetValue(tab.Type, out var list))
                {
                    list = new List<ProductView>();
                    _viewDictionary.Add(tab.Type, list);
                } 
                if (tab.Type == PeriodicType.Resources)
                {
                    tab.AddPart(_verticalContainer);
                    foreach (var product in products)
                    {
                        var view = _verticalPool.Get();
                        view.Setup(product);
                        list.Add(view);
                    }
                }
                else
                {
                    var label = _resolver.Instantiate(_labelPrefab, _root);
                    _content.Add(label.Rect);
                    tab.SetLabel(label);
                    var timer = _resolver.Instantiate(_timerPrefab, _root);
                    _content.Add(timer.Rect);
                    tab.SetTimer(timer);
                    
                    foreach (var product in products)
                    {
                        var view = _horizontalPool.Get();
                        view.Setup(product);
                        list.Add(view);
                        _content.Add(view.Rect);
                    }
                }
                tab.Initialize();
            }
            _content.Add(_root);
            for (var i = _content.Count - 1; i >= 0; i--)
                _content[i].SetSiblingIndex(0);
        }
        
        private void CheckVisibility(Vector2 pos)
        {
            var shownSet = new HashSet<string>(_analytics.GetProductsShowedProducts()); // O(n) единожды
            foreach (var kvp in _viewDictionary)
            {
                foreach (var view in kvp.Value)
                {
                    if (shownSet.Contains(view.Product.Id)) // O(1) проверка
                        continue;

                    if (IsRectVisible(view.Rect))
                        _analytics.SendPeriodicOfferShown(view.Product);
                }
            }
        }
        
        private bool IsRectVisible(RectTransform rectTransform)
        {
            Bounds viewportBounds = GetWorldBounds(_scroll.viewport);
            Bounds itemBounds = GetWorldBounds(rectTransform);
            return viewportBounds.Intersects(itemBounds);
        }
        
        private Bounds GetWorldBounds(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Bounds bounds = new Bounds(corners[0], Vector3.zero);
            
            for (int i = 1; i < 4; i++)
                bounds.Encapsulate(corners[i]);

            return bounds;
        }
        
        private void OnProductsRefreshed(PeriodicType type)
        {
            var views = _viewDictionary[type];
            var pool = type == PeriodicType.Resources ? _verticalPool : _horizontalPool;
            foreach (var view in views)
            {
                _content.Remove(view.Rect);
                pool.Release(view);
            } 
            views.Clear();

            var tab = _tabs.FirstOrDefault(item => item.Type == type);
            if (tab == null)
            {
                Logger.LogError($"Can`t find Tab by type{type}");
                return;
            }

            var lastRect = tab.GetLastRect();

            var index = _content.IndexOf(lastRect);
            var products = _productService.PeriodicProducts.GetActualProducts(tab.Type);
            foreach (var product in products)
            {
                var view = pool.Get();
                view.Setup(product);
                views.Add(view);
                if (type == PeriodicType.Resources) continue;
                _content.Insert(index + 1, view.Rect);
                index++;
            }

            if (type == PeriodicType.Resources)
            {
                for (var i = views.Count - 1; i >= 0; i--)
                    views[i].transform.SetSiblingIndex(0);
            }
            
            for (var i = _content.Count - 1; i >= 0; i--)
                _content[i].SetSiblingIndex(0);
            
            OnNewProducts?.Invoke(type);
        }

        private void OnButtonClicked(PeriodicType type) =>
            Select(type);

        private void Select(PeriodicType type)
        {
            SetType(type);
            foreach (var tab in _tabs)
            {
                if (_selectedType != tab.Type) continue;
                ScrollToCategory(tab.GetFirstRect());
                break;
            }
        }

        private void SetType(PeriodicType type)
        {
            _selectedType = type;
            foreach (var button in _buttons)
            {
                if(_selectedType == button.Type)
                    button.Select();
                else
                    button.Unselect();
            }
        }

        public override void Show()
        {
            base.Show();

            if (_selectedType == PeriodicType.None)
                Select(_defaultType);
        }

        private void Update()
        {
            var content = _scroll.content;
            var viewport = _scroll.viewport;
            var middlePoint = content.anchoredPosition.y + viewport.rect.height / 2;
            foreach (var tab in _tabs)
            {
                var position = -tab.StartPosition;
                if (content.anchoredPosition.y <= position && middlePoint >= position)
                {
                    if (_selectedType == tab.Type) return;
                    SetType(tab.Type);
                    break;
                }
            }
        }

        public void ScrollToCategory(RectTransform category)
        {
            _scroll.content.anchoredPosition = new Vector2(0, -category.anchoredPosition.y);
        }

        private ObjectPool<ProductView> CreatePool(ProductView prefab, RectTransform container)
        {
            return new ObjectPool<ProductView>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy);

            ProductView CreateFunc() => 
                _resolver.Instantiate(prefab, container);

            void ActionOnGet(ProductView view) => 
                view.gameObject.SetActive(true);

            void ActionOnRelease(ProductView view) => 
                view.gameObject.SetActive(false);

            void ActionOnDestroy(ProductView view) => 
                Destroy(view.gameObject);
        }

    }
}