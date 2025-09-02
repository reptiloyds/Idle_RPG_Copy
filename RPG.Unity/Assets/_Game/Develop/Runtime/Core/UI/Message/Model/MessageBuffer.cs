using System;
using System.Collections.Generic;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Message.Model
{
    public sealed class MessageBuffer
    {
        private readonly List<string> _activeMessages = new();
        private readonly List<string> _buffer = new();

        public IReadOnlyList<string> Buffer => _buffer;
        public event Action OnNewMessage;

        [Preserve]
        public MessageBuffer() { }

        public void Send(string message)
        {
            if(_activeMessages.Contains(message)) return;
            if(_buffer.Contains(message)) return;
            
            _buffer.Add(message);
            OnNewMessage?.Invoke();
        }

        public void Activate(string message)
        {
            _activeMessages.Add(message);
            _buffer.Remove(message);
        }

        public void Complete(string message) => 
            _activeMessages.Remove(message);
    }
}