using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Branches.Sheet;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Sheet
{
    public class CharacterContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Characters")] public BranchSheet branch { get; private set; }
        [Preserve] [SheetList("Characters")] public CharacterSheet characters { get; private set; }
        [Preserve] [SheetList("Characters")] public StuffSlotSheet stuffSlots { get; private set; }

        public CharacterContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            
            Balance.Set(characters);
            Balance.Set(branch);
            Balance.Set(stuffSlots);
        }
    }
}