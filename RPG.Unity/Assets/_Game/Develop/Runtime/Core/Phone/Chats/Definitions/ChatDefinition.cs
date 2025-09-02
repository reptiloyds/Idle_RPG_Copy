using System;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Definitions
{
    [Serializable]
    public class ChatDefinition
    {
        [Min(0)] public float PrintDelay = 0.5f;
    }
}