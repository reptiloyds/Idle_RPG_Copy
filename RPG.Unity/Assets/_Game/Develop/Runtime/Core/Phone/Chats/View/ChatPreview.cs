using System;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Type;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Accent.View;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.View
{
    [DisallowMultipleComponent]
    public class ChatPreview : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _lastMessage;
        [SerializeField] private TextMeshProUGUI _dateTime;
        [SerializeField] private int _lastMessageLength = 25;
        [SerializeField] private BaseButton _button;
        [SerializeField] private AccentSettingsHolder _accentSettings;
        [SerializeField] private PointerSettingsHolder _pointerSettings;

        [Inject] private ITranslator _translator;
        
        private ChatCharacter _character;
        public ChatCharacter Character => _character;

        public event Action<ChatPreview> OnClick;

        private void Awake() => 
            _button.OnClick += OnButtonClick;

        private void OnDestroy() => 
            _button.OnClick -= OnButtonClick;

        private void OnButtonClick() => 
            OnClick?.Invoke(this);

        public void Setup(ChatCharacter character, int index)
        {
            _character = character;
            _image.sprite = character.Sprite;
            _name.SetText(_character.Name);
            _character.Chat.LastMessage
                .Subscribe(ApplyMessage)
                .AddTo(this);
            _character.Chat.LastMessageTime
                .Subscribe(ApplyLastMessageTime)
                .AddTo(this);
            
            _button.ChangeButtonId($"chat_{index}");
        }

        public void SetupTutorialSettings(RectTransform windowRect)
        {
            _accentSettings.SetParent(windowRect);
            _pointerSettings.SetTarget(windowRect);
        }

        private void ApplyMessage(MessageData messageData)
        {
            string actor = null;
            switch (messageData.Actor)
            {
                case ActorType.Player:
                    actor = _translator.Translate(TranslationConst.You);
                    break;
                case ActorType.Converser:
                    actor = _character.Name;
                    break;
                default:
                    _lastMessage.SetText(string.Empty);
                    return;
            }
            switch (messageData.Type)
            {
                case MessageType.Text:
                    var message = _translator.Translate(messageData.Key);
                    if (string.IsNullOrEmpty(message))
                    {
                        _lastMessage.SetText(string.Empty);
                        return;
                    }

                    var lastMessage = $"{actor}: {message}";
                    if (lastMessage.Length > _lastMessageLength) 
                        lastMessage = lastMessage.Substring(0, _lastMessageLength);
                    _lastMessage.SetText(lastMessage);
                    break;
                case MessageType.Photo:
                    _lastMessage.SetText($"{actor}: {_translator.Translate(TranslationConst.ChatPhoto)}");
                    break;
            }
        }

        private void ApplyLastMessageTime(DateTime dateTime)
        {
            if (dateTime == default)
            {
                _dateTime.SetText(string.Empty);
                return;   
            }
            _dateTime.SetText($"{dateTime:MM/dd} {dateTime:HH:mm}");
        }
    }
}