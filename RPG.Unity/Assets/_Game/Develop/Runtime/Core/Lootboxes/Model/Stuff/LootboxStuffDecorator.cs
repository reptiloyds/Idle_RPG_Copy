using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model.Decorator;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model.Stuff
{
    public class LootboxStuffDecorator : LootboxDecorator
    {
        private readonly StuffInventory _inventory;
        private readonly BranchService _branchService;

        public LootboxStuffDecorator(StuffInventory inventory,
            BranchService branchService, ILootboxDecorator decorator = null) : base(decorator)
        {
            _inventory = inventory;
            _branchService = branchService;
        }

        public override bool IsItemAvailable(Item item) => 
            base.IsItemAvailable(item) && IsItemAvailableInternal(item);

        private bool IsItemAvailableInternal(Item item)
        {
            var stuffItem = _inventory.GetItem(item.Id);
            foreach (var slot in _inventory.Slots)
            {
                if(slot.Type != stuffItem.SlotType) continue;
                if (IsBranchAvailable(slot)) return true;
            }

            return false;
        }
        
        private bool IsBranchAvailable(StuffSlot slot) => 
            _branchService.GetBranch(slot.BranchId).IsUnlocked;
    }
}