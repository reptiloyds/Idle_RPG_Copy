using System.Collections;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Deal.Controller;
using PleasantlyGames.RPG.Runtime.Core.Deal.View;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ChatView : MonoBehaviour
    {
        [SerializeField] private Image _converserImage;
        [SerializeField] private TextMeshProUGUI _converserName;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private ChatInfinityScroller _infinityScroller;
        [SerializeField] private GameObject _startView;
        [SerializeField] private TextMeshProUGUI _conditionText;
        [SerializeField] private BaseButton _startButton;
        [SerializeField] private BaseButton _scrollDownButton;
        [SerializeField] private ResourceDealView _resourceStartButton;
        [SerializeField] private List<ChatAnswerButton> _answerButtons;

        [Inject] private IObjectResolver _resolver;
        [Inject] private ResourceService _resourceService;
        [Inject] private MessageBuffer _messageBuffer;
        [Inject] private ITranslator _translator;

        private ResourceDealController _dealController;
        private UnityAction<Vector2> _scrollAction;
        private bool _isScrollDownShowed;
            
        private CompositeDisposable _compositeDisposable;
        private ChatCharacter _character;
        private readonly List<MessageData> _messages = new();
        private bool _needUpdateScroll;
        
        public bool IsActive { get; private set; }

        private void Awake()
        {
            _scrollAction = OnScroll;
            _resourceStartButton.SetLabelText(_translator.Translate(TranslationConst.ChatStart));
            _dealController = new ResourceDealController(_resourceStartButton, _resourceService, _messageBuffer, _translator);
            _dealController.OnSuccess += OnSuccessDeal;
            _startButton.OnClick += StartConversation;
            _scrollDownButton.OnClick += OnScrollDownClick;

            foreach (var answerButton in _answerButtons) 
                answerButton.OnSelected += OnAnswerSelected;
            
            HidePlayerVariants();
            HideStartView();
            _infinityScroller.Initialize();
        }

        private void OnDestroy()
        {
            _dealController.OnSuccess -= OnSuccessDeal;
            _startButton.OnClick -= StartConversation;
            _scrollDownButton.OnClick -= OnScrollDownClick;
            foreach (var answerButton in _answerButtons) 
                answerButton.OnSelected -= OnAnswerSelected;
            if(_character != null)
                Clear();
        }

        private void OnScrollDownClick() => 
            _infinityScroller.ScrollToBottom();

        public void Show(ChatCharacter character)
        {
            IsActive = true;
            gameObject.SetActive(true);

            if (_character == character)
                return;
            if(_character != null)
                Clear();
            
            _compositeDisposable = new CompositeDisposable();
            _character = character;
            _dealController.SetInteractionCondition(_character.Conditions.IsAllConditionCompleted);
            _converserImage.sprite = _character.Sprite;
            _converserName.SetText(_character.Name);
            
            foreach (var message in _character.Chat.PostedMessages) 
                PostMessage(message);
            
            
            if (!string.IsNullOrEmpty(_character.Chat.TypingMessage.CurrentValue.Key)) 
                OnMessageTyping(_character.Chat.TypingMessage.CurrentValue);
            _compositeDisposable.Add(_character.Chat.TypingMessage
                .Skip(1)
                .Where(value => !string.IsNullOrEmpty(value.Key))
                .Subscribe(OnMessageTyping));
            _character.Chat.OnMessagePosted += OnMessagePosted;
            _character.Chat.OnPlayerVariantsOffered += ShowPlayerVariants;
            if (_character.Chat.PlayerVariants.Count > 0) 
                ShowPlayerVariants();
            else
                HidePlayerVariants();


            _character.Conditions.OnUpdated += UpdateStartView;
            UpdateStartView();
            
            _scrollDownButton.gameObject.SetActive(false);
            _scrollRect.onValueChanged.AddListener(_scrollAction);
            HideScrollDown();
        }

        public void Hide()
        {
            IsActive = false;
            gameObject.SetActive(false);
            _scrollRect.onValueChanged.RemoveListener(_scrollAction);
            Clear();
        }

        private void OnScroll(Vector2 scrollDelta)
        {
            if (_infinityScroller.IsReadyToScrollDown())
            {
                if(!_isScrollDownShowed)
                    ShowScrollDown();
            }
            else if(_isScrollDownShowed)
                    HideScrollDown();
        }

        private void ShowScrollDown()
        {
            _isScrollDownShowed = true;
            _scrollDownButton.gameObject.SetActive(true);
        }

        private void HideScrollDown()
        {
            _isScrollDownShowed = false;
            _scrollDownButton.gameObject.SetActive(false);
        }

        private void Clear()
        {
            _infinityScroller.Clear();
            
            _character.Conditions.OnUpdated -= UpdateStartView;
            _character.Chat.OnMessagePosted -= OnMessagePosted;
            _character.Chat.OnPlayerVariantsOffered -= ShowPlayerVariants;
            _character = null;
            _compositeDisposable?.Dispose();
        }

        public void DisableIfActive() => 
            gameObject.SetActive(false);

        private void UpdateStartView()
        {
            if (_character.Chat.State.CurrentValue == ChatState.Awaiting)
            {
                _startView.gameObject.SetActive(true);
                _conditionText.gameObject.SetActive(true);
                _conditionText.SetText(_character.Conditions.Description.CurrentValue);
                var price = _character.Conditions.GetPrice();
                if (price.amount == 0)
                {
                    _startButton.gameObject.SetActive(true);
                    _startButton.SetInteractable(_character.Conditions.IsAllConditionCompleted());
                    _resourceStartButton.gameObject.SetActive(false);
                }
                else
                {
                    _startButton.gameObject.SetActive(false);
                    _resourceStartButton.gameObject.SetActive(true);
                    _dealController.ClearPrice();
                    _dealController.AddPrice(price.type, price.amount);
                    _dealController.BuildPrice();
                }   
            }
            else
                HideStartView();
        }

        private void HideStartView() => 
            _startView.SetActive(false);

        private void OnSuccessDeal(ResourceDealController dealController) => 
            StartConversation();

        private void StartConversation()
        {
            HideStartView();
            _character.Chat.StartConversation();
        }

        private void ShowPlayerVariants()
        {
            var variants = _character.Chat.PlayerVariants;
            for (var i = 0; i < _answerButtons.Count; i++)
            {
                if (i < variants.Count)
                {
                    _answerButtons[i].gameObject.SetActive(true);
                    _answerButtons[i].SetVariant(variants[i]);
                }
                else
                    _answerButtons[i].gameObject.SetActive(false);
            }
        }

        private void HidePlayerVariants()
        {
            foreach (var answerButton in _answerButtons)
                answerButton.gameObject.SetActive(false);
        }

        private void OnAnswerSelected(string variantKey)
        {
            HidePlayerVariants();
            _character.Chat.ChoosePlayerVariant(variantKey);
        }

        private void OnMessageTyping(MessageData data)
        {
            _infinityScroller.AddMessage(new ChatMessageView.ChatMessageData()
            {
                Message = data,
                Converser = _character,
                IsNew = true,
                IsTyping = true
            });
        }

        private void OnMessagePosted(MessageData data)
        {
            _infinityScroller.RemoveLast();
            PostMessage(data);
        }

        private void PostMessage(MessageData data)
        {
            var isNew = false;
            if (_messages.Count == 0) 
                isNew = true;
            else if (_messages[^1].Actor != data.Actor) 
                isNew = true;
            _infinityScroller.AddMessage(new ChatMessageView.ChatMessageData()
            {
                Message = data,
                Converser = _character,
                IsNew = isNew
            });
            _messages.Add(data);
        }

        private void Update()
        {
            if (!_needUpdateScroll) return;
            StartCoroutine(UpdateScrollRect());
            _needUpdateScroll = false;
        }

        private IEnumerator UpdateScrollRect()
        {
            yield return null;
            _scrollRect.verticalNormalizedPosition = 0;   
        }
    }
}