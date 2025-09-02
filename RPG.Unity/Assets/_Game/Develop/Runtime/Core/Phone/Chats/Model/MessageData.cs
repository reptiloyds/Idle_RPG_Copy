using System;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model
{
    [Serializable]
    public struct MessageData
    {
        public ActorType Actor;
        public MessageType Type;
        public string Key;

        public MessageData(ActorType actor, MessageType type, string key)
        {
            Actor = actor;
            Type = type;
            Key = key;
        }
    }
}