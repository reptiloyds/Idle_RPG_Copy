using System;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Phone
{
    public class ChatAnswersShowCondition : TutorialCondition
    {
        private readonly ChatService _chat;
        private readonly string _chatId;

        public ChatAnswersShowCondition(ChatService chat, string chatId)
        {
            _chat = chat;
            _chatId = chatId;
        }
        
        public override void Initialize()
        {
            base.Initialize();

            _chat.OnAnswersShown += TryComplete;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _chat.OnAnswersShown -= TryComplete;
        }

        public override void Pause() => 
            _chat.OnAnswersShown -= TryComplete;

        public override void Continue() => 
            _chat.OnAnswersShown += TryComplete;

        private void TryComplete(string chatId)
        {
            if(!String.Equals(_chatId, chatId)) return;

            _chat.OnAnswersShown -= TryComplete;
            Complete();
        }
    }
}