using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Sheet;
using UnityEngine.Scripting;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Sheet
{
    public class PhoneContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Phone")] public ChatSheet chats { get; private set; }
        [Preserve] [SheetList("Phone")] public ChatCharactersSheet chatCharacters { get; private set; }
        [Preserve] [SheetList("Phone")] public GallerySheet gallery { get; private set; }
        
        [Preserve]
        public PhoneContainer(ILogger logger) : base(logger) { }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            
            Balance.Set(chats);
            Balance.Set(chatCharacters);
            Balance.Set(gallery);
        }
    }
}