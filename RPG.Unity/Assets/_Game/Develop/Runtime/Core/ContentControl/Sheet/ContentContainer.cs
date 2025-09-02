using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Sheet
{
    public class ContentContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("ContentControl")] public ContentSheet content { get; private set; }
        [Preserve] [SheetList("Products")] public ContentSheet productsContent { get; private set; }

        public ContentContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();

            foreach (var row in productsContent) 
                content.Add(row);
            Balance.Set(content);
        }
    }
}