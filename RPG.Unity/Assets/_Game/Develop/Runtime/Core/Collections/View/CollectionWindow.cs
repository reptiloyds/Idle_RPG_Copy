using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Collections.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Collections.View
{
    public class CollectionWindow : BaseWindow
    {
        [Serializable]
        private class CollectionPageConfig
        {
            public CollectionPageView View;
            public BookmarkButton Button;
        }
        
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField] private ItemType _defaultPage;
        [SerializeField] private List<CollectionPageConfig> _pageConfigs;
        [SerializeField] private CollectionView _view;
        
        [Inject] private CollectionService _service;
        [Inject] private IObjectResolver _resolver;

        private readonly List<CollectionView> _views = new();
        private CollectionPageConfig _selectedConfig;

        protected override void Awake()
        {
            base.Awake();
            
            foreach (var pageConfig in _pageConfigs) 
                pageConfig.Button.OnBookmarkClicked += OnClick; 
            
            CreateViews();
            
            foreach (var config in _pageConfigs)
            {
                if (config.View.Type != _defaultPage) 
                    config.View.gameObject.SetActive(false);
                else
                    Select(config);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (var pageConfig in _pageConfigs) 
                pageConfig.Button.OnBookmarkClicked -= OnClick;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        public CollectionView GetViewByModel(Collection collection)
        {
            foreach (var view in _views)
            {
                if(view.Collection == collection)
                    return view;
            }

            return null;
        }

        public BaseButton GetBookmarkButton(ItemType itemType)
        {
            foreach (var config in _pageConfigs)
            {
                if (config.View.Type == itemType)
                    return config.Button;
            }

            return null;
        }

        private void OnClick(BookmarkButton bookmarkButton)
        {
            foreach (var config in _pageConfigs)
            {
                if (config.Button != bookmarkButton) continue;
                Select(config);
                return;
            }
        }

        private void Select(CollectionPageConfig config)
        {
            if(_selectedConfig == config) return;
            
            if (_selectedConfig != null)
            {
                _selectedConfig.Button.Deselect();
                _selectedConfig.View.gameObject.SetActive(false);
            }
            _selectedConfig = config;
            
            _selectedConfig.Button.Select();
            _selectedConfig.View.gameObject.SetActive(true);
        }

        private void CreateViews()
        {
            var collections = _service.Collections;
            var pageDictionary = _pageConfigs.ToDictionary(x => x.View.Type);
            foreach (var collection in collections)
            {
                if (pageDictionary.TryGetValue(collection.Type, out var page))
                {
                    var view = _resolver.Instantiate(_view, page.View.Container);
                    view.Setup(collection);
                    view.OnEnhanceClicked += OnEnhanceClicked;
                    page.View.AppendView(view);
                    _views.Add(view);
                }
            }
        }

        private void OnEnhanceClicked(CollectionView collectionView) => 
            _service.Enhance(collectionView.Collection);
    }
}