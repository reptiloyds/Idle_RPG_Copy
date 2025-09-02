using System;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Type;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.View;
using PleasantlyGames.RPG.Runtime.Core.UI.InfinityScrolling;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.View
{
    public class ChatMessageView : InfinityScrollElement<ChatMessageView.ChatMessageData>
    {
        private enum Alignment
        {
            Left = 0,
            Right = 1,
        }

        [SerializeField] private RectTransform _avatarContainer;
        [SerializeField] private RectTransform _contentContainerRect;
        [SerializeField] [Min(0)] private float _widthOffset = 100;
        [SerializeField] [Min(0)] private float _avatarHorizontalOffset = 10;
        [SerializeField] [Min(0)] private float _avatarTopOffset = 35;
        [SerializeField] [Min(0)] private float _photoAspectRatio = 2;
        [SerializeField] private TextMeshProUGUI _textContent;
        [SerializeField] private PhotoView _photoView;
        [SerializeField] private Image _converserImage;
        [SerializeField] private Image _contentContainerImage;
        [SerializeField] private Color _converserColor;
        [SerializeField] private Color _playerColor;
        [SerializeField] private bool _showAvatar = true;
        [SerializeField] private bool _toolInstance;

        [SerializeField] [FoldoutGroup("Animation")] private float _typeAnimationInterval = 0.25f;

        private IDisposable _animationDisposable;
        private const int DotsCount = 3;
        private string[] _animationTexts;
        private static string[] _typingTexts;
        private static string[] _sendingPhotoTexts;

        [Inject] private ITranslator _translator;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private GalleryService _galleryService;
        
        [ShowInInspector, HideInEditorMode, ReadOnly]
        private Alignment _alignment;

        public struct ChatMessageData
        {
            public MessageData Message;
            public ChatCharacter Converser;
            public bool IsNew;
            public bool IsTyping;
        }
        
        private ChatMessageData _data;

        private void Awake()
        {
            if (_typingTexts == null)
            {
                var typingText = _translator.Translate(TranslationConst.ChatTyping);
                _typingTexts = new string[DotsCount + 1];
                for (var i = 0; i <= DotsCount; i++) 
                    _typingTexts[i] = typingText + new string('.', i);
            }

            if (_sendingPhotoTexts == null)
            {
                var sendingPhotoText = _translator.Translate(TranslationConst.ChatSendingPhoto);
                _sendingPhotoTexts = new string[DotsCount + 1];
                for (var i = 0; i <= DotsCount; i++)
                    _sendingPhotoTexts[i] = sendingPhotoText + new string('.', i);
            }
        }
        
        public override void Setup(ChatMessageData data)
        {
            _data = data;
            if(_animationDisposable != null)
                StopPrintAnimation();

            if (_data.IsTyping) 
                SetupAnimation();
            else
                SetupMessage();
        }

        private void SetupMessage()
        {
            switch (_data.Message.Type)
            {
                case MessageType.Text:
                    _animationTexts = _typingTexts;
                    break;
                case MessageType.Photo:
                    _animationTexts = _sendingPhotoTexts;
                    break;
            }

            SelectContent(_data.Message.Type);
            SetupContent();
            UpdateOrder();
            UpdateByConverser(_data.Message.Actor);
            UpdateRectSizes(_data.Message.Type);
        }

        private void SetupAnimation()
        {
            switch (_data.Message.Type)
            {
                case MessageType.Text:
                    _animationTexts = _typingTexts;
                    break;
                case MessageType.Photo:
                    _animationTexts = _sendingPhotoTexts;
                    break;
            }
            
            _textContent.SetText(_animationTexts[^1]);
            
            SelectContent(MessageType.Text);
            UpdateOrder();
            UpdateByConverser(_data.Message.Actor);
            UpdateRectSizes(MessageType.Text);

            PlayPrintAnimation();
        }

        private void PlayPrintAnimation()
        {
            var dotCount = 0;
            _animationDisposable = Observable
                .Interval(TimeSpan.FromSeconds(_typeAnimationInterval), UnityTimeProvider.InitializationIgnoreTimeScale)
                .ObserveOnCurrentSynchronizationContext()
                .Subscribe(_ =>
                {
                    _textContent.SetText(_animationTexts[dotCount]);
                    dotCount = (dotCount + 1) % _animationTexts.Length;
                });
        }

        private void StopPrintAnimation() => 
            _animationDisposable?.Dispose();

        private void OnDisable()
        {
            StopPrintAnimation();
            _photoView.Clear();
        }

        private void SelectContent(MessageType type)
        {
            _textContent.gameObject.SetActive(type == MessageType.Text);
            _photoView.gameObject.SetActive(type == MessageType.Photo);
        }

        private void SetupContent()
        {
            switch (_data.Message.Type)
            {
                case MessageType.Text:
                    _textContent.SetText(_translator.Translate(_data.Message.Key));
                    break;
                case MessageType.Photo:
                    if (!_toolInstance)
                        _photoView.Setup(_galleryService.GetPhoto(_data.Message.Key)); 
                    break;
            }
        }

        private void UpdateOrder()
        {
            if (transform.parent.childCount <= 1)
                return;
            transform.SetAsLastSibling();
        }

        private void UpdateByConverser(ActorType actor)
        {
            switch (actor)
            {
                case ActorType.Converser:
                    _avatarContainer.gameObject.SetActive(_data.IsNew && _showAvatar);
                    if (_data.IsNew) 
                        _converserImage.sprite = _data.Converser.Sprite;
                    _alignment = Alignment.Left;
                    _contentContainerImage.color = _converserColor;
                    break;
                default:
                    _avatarContainer.gameObject.SetActive(false);
                    _alignment = Alignment.Right;
                    _contentContainerImage.color = _playerColor;
                    break;
            }
        }

        private void UpdateRectSizes(MessageType type)
        {
            var parentWidth = _selfRect.parent.GetComponent<RectTransform>().rect.width;
            var maxWidth = parentWidth - _widthOffset - (_showAvatar ? _avatarContainer.rect.width + _avatarHorizontalOffset : 0);

            switch (type)
            {
                case MessageType.Text:
                    RectResizeByTextContent(maxWidth);
                    break;
                case MessageType.Photo:
                    RectResizeByPhotoContent(maxWidth);
                    break;
            }

            ApplyAlignment();
        }

        private void RectResizeByTextContent(float maxWidth)
        {
            var preferredContentValues = _textContent.GetPreferredValues(_textContent.text, maxWidth, 0);
            var preferredWidth = preferredContentValues.x + GetHorizontalPaddings(_textContent.rectTransform);
            if (preferredWidth >= maxWidth)
            {
                _contentContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
                _textContent.ForceMeshUpdate(forceTextReparsing: true);
            }
            else
            {
                _contentContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredWidth);
                _textContent.ForceMeshUpdate();
            }

            var verticalPaddings = GetVerticalPaddings(_textContent.rectTransform);
            var renderedValues = _textContent.GetRenderedValues();
            _contentContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, renderedValues.y + verticalPaddings);
            _selfRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, renderedValues.y + verticalPaddings + GetAvatarTopOffset());
        }

        private void RectResizeByPhotoContent(float maxWidth)
        {
            var horizontalPaddings = GetHorizontalPaddings(_photoView.SelfRect);
            var verticalPaddings = GetVerticalPaddings(_photoView.SelfRect);
            _contentContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
            var photoWidth = maxWidth - horizontalPaddings;
            var photoHeight = photoWidth / _photoAspectRatio;
            _contentContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, photoHeight + verticalPaddings);
            _selfRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, photoHeight + verticalPaddings + GetAvatarTopOffset());
        }

        private void ApplyAlignment()
        {
            switch (_alignment)
            {
                case Alignment.Left:
                    _contentContainerRect.pivot = new Vector2(0, 1);
                    _contentContainerRect.anchorMin = new Vector2(0, 1);
                    _contentContainerRect.anchorMax = new Vector2(0, 1);
                    _contentContainerRect.anchoredPosition = new Vector2(GetAvatarHorizontalOffset(), -GetAvatarTopOffset());
                    break;
                case Alignment.Right:
                    _contentContainerRect.pivot = new Vector2(1, 1);
                    _contentContainerRect.anchorMin = new Vector2(1, 1);
                    _contentContainerRect.anchorMax = new Vector2(1, 1);
                    _contentContainerRect.anchoredPosition = new Vector2(0, -GetAvatarTopOffset());
                    break;
            }
        }

        private float GetVerticalPaddings(RectTransform rectTransform) => 
            rectTransform.offsetMin.y - rectTransform.offsetMax.y;
        
        private float GetHorizontalPaddings(RectTransform rectTransform) =>
            rectTransform.offsetMin.x - rectTransform.offsetMax.x;

        private float GetAvatarTopOffset() => 
            _avatarContainer.gameObject.activeSelf ? _avatarTopOffset : 0;

        private float GetAvatarHorizontalOffset() => 
            _avatarContainer.gameObject.activeSelf ? _avatarContainer.rect.width + _avatarHorizontalOffset : 0;
    }
}