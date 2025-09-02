using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.Sheets
{
    public class ResourceContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Resources")] public ResourceSheet resource { get; private set; }
        
        public ResourceContainer(ILogger logger) : base(logger)
        {
        }

        public override void PostLoad()
        {
            base.PostLoad();

            if (Balance == null) return;
            
            Balance.Set(resource);
        }
    }
}