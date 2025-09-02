using System;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Type;
using UnityEngine.Scripting;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Sheet
{
    public class ChatSheet : Sheet<string, ChatSheet.Row>
    {
        [Preserve]
        public ChatSheet() { }
        
        public class Row : SheetRowArray<string, Elem>
        {
            [Preserve] public ChatConditionData Condition { get; private set; }
            
            [Preserve]
            public Row() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate() => 
                Condition.Validate();
        }
        
        public class Elem : SheetRowElem
        {
            [Preserve] public int Step { get; private set; }
            [Preserve] public int NextStep { get; private set; }
            [Preserve] public ActorType Actor { get; private set; }
            [Preserve] public string Variant { get; private set; }
            [Preserve] public MessageType MessageType { get; private set; }
            [Preserve] public string MessageKey { get; private set; }
                
            [Preserve]
            public Elem() { }

            public MessageData ConvertToData()
            {
                return new MessageData()
                {
                    Actor = Actor,
                    Type = MessageType,
                    Key = MessageKey
                };
            }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                if (Step < 1) 
                    Logger.LogError("Step must be greater than 0");
                if(Actor == ActorType.None)
                    Logger.LogError("Actor type must be set");
                
                if(string.IsNullOrEmpty(MessageKey))
                    Logger.LogError("Message key must be set");
            }
        }
    }

    [Serializable]
    public class ChatConditionData
    {
        [Preserve] public int Price { get; private set; }
        [Preserve] public int CharacterLevel { get; private set; }
        [Preserve] public int StageId { get; private set; }
        [Preserve] public int StageLevel { get; private set; }
        
        [Preserve]
        public ChatConditionData() { }

        public void Validate()
        {
            if(Price < 0)
                Logger.LogError("Price must be greater than 0");
            
            if(CharacterLevel < 0)
                Logger.LogError("Character level must be greater than 0");
        }
    }
}