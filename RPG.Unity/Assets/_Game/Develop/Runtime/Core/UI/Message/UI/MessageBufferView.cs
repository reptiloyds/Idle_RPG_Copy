using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Message.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    public class MessageBufferView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField, Required] private Message _messageTemplate;
        [SerializeField, Required] private Transform _messageContainer;
        [SerializeField, Required] private Transform _poolContainer;
        [SerializeField, MinValue(1)] private int _maxMessage = 5;

        private readonly List<Message> _activeMessages = new();
        private ObjectPool<Message> _pool;

        [Inject] private MessageBuffer _messageBuffer;

        private int FreePlaces => _maxMessage - _activeMessages.Count;

        void IInitializable.Initialize()
        {
            _pool = new ObjectPool<Message>(CreateMessage, EnableMessage, DisableMessage);
            _messageBuffer.OnNewMessage += TryShowMessage;
            TryShowMessage();
        }

        void IDisposable.Dispose()
        {
            _messageBuffer.OnNewMessage -= TryShowMessage;
            foreach (var activeMessage in _activeMessages) 
                activeMessage.OnComplete -= OnComplete;
        }

        private void TryShowMessage()
        {
            var messageAmount = _messageBuffer.Buffer.Count;
            for (var i = 0; i < messageAmount; i++)
            {
                var message = _messageBuffer.Buffer[0];
                if(FreePlaces == 0) return;
                _messageBuffer.Activate(message);
                Show(message);   
            }
        }

        private void Show(string text)
        {
            var message = _pool.Get();
            _activeMessages.Add(message);
            message.Show(text);
            message.OnComplete += OnComplete;
        }

        private void OnComplete(Message message)
        {
            message.OnComplete -= OnComplete;
            _activeMessages.Remove(message);
            _pool.Release(message);
            _messageBuffer.Complete(message.Content);
            
            TryShowMessage();
        }

        private Message CreateMessage() => 
            Instantiate(_messageTemplate, _poolContainer);

        private void EnableMessage(Message message)
        {
            message.gameObject.SetActive(true);
            message.transform.SetParent(_messageContainer);
        }

        private void DisableMessage(Message message)
        {
            message.gameObject.SetActive(false);
            message.transform.SetParent(_poolContainer);
        }
    }
}