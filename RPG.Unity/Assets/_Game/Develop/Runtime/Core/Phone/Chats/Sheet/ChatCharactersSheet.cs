using Cathei.BakingSheet;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Sheet
{
    public class ChatCharactersSheet : Sheet<string, ChatCharactersSheet.Row>
    {
        [Preserve]
        public ChatCharactersSheet() { }
        
        public class Row : SheetRow<string>
        {
            [Preserve] public string ImageName { get; private set; }
            
            [Preserve]
            public Row() { }
        }
    }
}