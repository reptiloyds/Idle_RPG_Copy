using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.View
{
    public class ChatPage : PhonePage
    {
        [SerializeField] private RectTransform _windowRect;
        [SerializeField] private ChatPreview _previewPrefab;
        [SerializeField] private RectTransform _previewContainer;
        [SerializeField] private GameObject _view;
        [SerializeField] private ChatView _chatView;
        [SerializeField] private List<RectTransform> _animationTargets;

        [Inject] private ChatService _chatService;
        [Inject] private IObjectResolver _resolver;

        private readonly List<ChatPreview> _previews = new();
        private int _chatIndex = 0;
        
        public override void Initialize()
        {
            base.Initialize();
            _chatView.DisableIfActive();
            CreatePreviews();
        }

        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, true, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, true, callback);

        public override void CloseSignal()
        {
            if (_chatView.IsActive)
            {
                _chatView.Hide();
                _view.SetActive(true);
            } 
            else
                Hide();
        }

        public ChatPreview GetPreviewByModel(ChatCharacter character)
        {
            foreach (var preview in _previews)
                if(preview.Character == character) return preview;
            return null;
        }

        private void OnDestroy()
        {
            foreach (var preview in _previews) 
                preview.OnClick -= OnPreviewClick;
        }

        private void CreatePreviews()
        {
            foreach (var character in _chatService.Characters)
            {
                var preview = CreatePreview(character);
                preview.gameObject.SetActive(character.IsUnlocked);
                
                if (!character.IsUnlocked) 
                    character.OnUnlocked += OnCharacterUnlocked;
            }
        }

        private void OnCharacterUnlocked(ChatCharacter character)
        {
            character.OnUnlocked -= OnCharacterUnlocked;

            var preview = GetPreviewByModel(character);
            if (preview != null) 
                preview.gameObject.SetActive(true);
        }

        private ChatPreview CreatePreview(ChatCharacter character)
        {
            var preview = _resolver.Instantiate(_previewPrefab, _previewContainer);
            preview.Setup(character, _chatIndex++);
            preview.SetupTutorialSettings(_windowRect);
            _previews.Add(preview);
            preview.OnClick += OnPreviewClick;

            return preview;
        }

        private void OnPreviewClick(ChatPreview preview)
        {
            _view.SetActive(false);
            _chatView.Show(preview.Character);
        }
    }
}