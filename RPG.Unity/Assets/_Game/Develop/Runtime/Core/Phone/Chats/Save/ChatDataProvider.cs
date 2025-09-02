using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Type;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Save
{
    [Serializable]
    public class DialogueDataContainer
    {
        public Dictionary<string, ChatCharacterData> ChatCharacters = new();

        [UnityEngine.Scripting.Preserve]
        public DialogueDataContainer()
        {
        }
    }

    [Serializable]
    public class ChatCharacterData
    {
        public ChatData ChatData = new();
    }

    [Serializable]
    public class ChatData
    {
        public ChatState ChatState = ChatState.Awaiting;
        public int Сonversation = 1;
        public int Step;
        public DateTime LastMessageTime;
        public List<MessageData> Messages = new();

        [UnityEngine.Scripting.Preserve]
        public ChatData()
        {
        }
    }

    public class ChatDataProvider : BaseDataProvider<DialogueDataContainer>
    {
        [Inject] private BalanceContainer _balanceContainer;

        [UnityEngine.Scripting.Preserve]
        public ChatDataProvider()
        {
        }

        public override void LoadData()
        {
            base.LoadData();

            if (Data == null)
                Data = CreateData();
            else
                ValidateData();
        }

        private DialogueDataContainer CreateData()
        {
            var sheet = _balanceContainer.Get<ChatCharactersSheet>();
            var data = new DialogueDataContainer();
            foreach (var config in sheet)
                data.ChatCharacters.Add(config.Id, new ChatCharacterData());

            return data;
        }

        private void ValidateData()
        {
            var sheet = _balanceContainer.Get<ChatCharactersSheet>();
            foreach (var config in sheet)
                if (!Data.ChatCharacters.ContainsKey(config.Id))
                    Data.ChatCharacters.Add(config.Id, new ChatCharacterData());

            var removeList = new List<string>(Data.ChatCharacters.Count);

            foreach (var kvp in Data.ChatCharacters)
                if (!sheet.Contains(kvp.Key))
                    removeList.Add(kvp.Key);

            foreach (var key in removeList)
                Data.ChatCharacters.Remove(key);
        }
    }
}