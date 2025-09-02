using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Collections.Sheets
{
    public class CollectionContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Collection")] public CollectionSheet collection { get; private set; }
        [Preserve] [SheetList("Collection")] public ManualFormulaSheet<CollectionSheet, string> manualEffects { get; private set; }

        public CollectionContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(collection);
            Balance.Set(manualEffects);
        }
    }
}