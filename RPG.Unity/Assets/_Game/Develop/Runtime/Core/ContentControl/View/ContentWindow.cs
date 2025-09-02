using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.View
{
    public class ContentWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private RectTransform _viewContainer;
        [SerializeField] private ContentView _viewPrefab;

        private List<ContentView> _views;
        
        [Inject] private ContentService _contentService;
        [Inject] private IObjectResolver _resolver;

        protected override void Awake()
        {
            base.Awake();
            CreateViews();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var view in _views) 
                view.OnReceiveClicked -= OnReceiveClicked;
            _views.Clear();
        }

        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        private void CreateViews()
        {
            var contents = _contentService.GetManualLockedContent();
            _views = new List<ContentView>(contents.Count);
            foreach (var content in contents) 
                CreateView(content);
        }

        private void CreateView(Content content)
        {
            var view = _resolver.Instantiate(_viewPrefab, _viewContainer);
            view.Setup(content);
            _views.Add(view);
            
            view.OnReceiveClicked += OnReceiveClicked;
        }

        private void OnReceiveClicked(ContentView contentView)
        {
            if(!contentView.Content.IsReadyForManualUnlock) return;
            var content = contentView.Content;
            content.ManualUnlock();
            if (content.IsUnlocked) 
                DestroyView(contentView);
        }

        private void DestroyView(ContentView contentView)
        {
            contentView.OnReceiveClicked -= OnReceiveClicked;
            _views.Remove(contentView);
            contentView.gameObject.SetActive(false);
        }
    }
}