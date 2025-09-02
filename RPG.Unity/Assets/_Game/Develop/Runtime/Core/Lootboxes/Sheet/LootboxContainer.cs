using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet
{
    public class LootboxContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Lootbox")] public LootboxSheet lootbox { get; private set; }

        public LootboxContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(lootbox);
        }
    }
}