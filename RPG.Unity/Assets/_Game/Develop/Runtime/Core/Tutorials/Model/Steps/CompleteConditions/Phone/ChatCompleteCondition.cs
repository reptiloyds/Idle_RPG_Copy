using System;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Phone
{
    public class ChatCompleteCondition : TutorialCondition
    {
        private readonly ChatService _chat;
        private readonly string _chatId;

        public ChatCompleteCondition(ChatService chat, string chatId)
        {
            _chat = chat;
            _chatId = chatId;
        }
        
        public override void Initialize()
        {
            base.Initialize();

            _chat.OnChatCompleted += TryComplete;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _chat.OnChatCompleted -= TryComplete;
        }

        public override void Pause() => 
            _chat.OnChatCompleted -= TryComplete;

        public override void Continue() => 
            _chat.OnChatCompleted += TryComplete;

        private void TryComplete(string chatId)
        {
            if(!String.Equals(_chatId, chatId)) return;

            _chat.OnChatCompleted -= TryComplete;
            Complete();
        }
    }
}