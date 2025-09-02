using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.View;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.View
{
    public class PhoneWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private List<PhonePage> _pages;
        [SerializeField] private BaseButton _chatButton;
        [SerializeField] private BaseButton _galleryButton;
        [SerializeField] private ChatPage _chatPage;
        [SerializeField] private GalleryPage _galleryPage;
        [SerializeField] private PhoneBottomButton _bottomButton;

        private PhonePage _currentPage;

        public GalleryPage GalleryPage => _galleryPage;
        public ChatPage ChatPage => _chatPage;
        public BaseButton ChatButton => _chatButton;
        public BaseButton GalleryButton => _galleryButton;

        private void Reset() => 
            GetComponents();

        private void OnValidate() => 
            GetComponents();

        [Button]
        private void GetComponents() => 
            _pages = GetComponentsInChildren<PhonePage>().ToList();

        protected override void Awake()
        {
            base.Awake();
            
            _chatButton.OnClick += OnChatClick;
            _galleryButton.OnClick += OnGalleryClick;

            foreach (var page in _pages) 
                page.Initialize();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _chatButton.OnClick -= OnChatClick;
            _galleryButton.OnClick -= OnGalleryClick;
        }

        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        private void OnChatClick() => 
            OpenPage(_chatPage);

        private void OnGalleryClick() => 
            OpenPage(_galleryPage);

        public override void Open()
        {
            base.Open();
            
            if(_currentPage == null)
                _bottomButton.SetCloseVisual();
            else
                _bottomButton.SetBackVisual();
        }

        protected override void OnCloseClick()
        {
            if (_currentPage == null)
                base.OnCloseClick();
            else
                _currentPage.CloseSignal();
        }

        private void OpenPage(PhonePage page)
        {
            _currentPage = page;
            _currentPage.OnHidden += OnPageHidden;
            _currentPage.Show();
            _bottomButton.SetBackVisual();
        }

        private void OnPageHidden()
        {
            _currentPage.OnHidden -= OnPageHidden;
            _currentPage = null;
            _bottomButton.SetCloseVisual();
        }
    }
}