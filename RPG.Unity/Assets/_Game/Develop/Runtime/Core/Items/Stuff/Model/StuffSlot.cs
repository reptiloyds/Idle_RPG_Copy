using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Branches.Sheet;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Tag;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model
{
    public class StuffSlot : Slot
    {
        private readonly StuffData _data;
        private readonly StuffSlotSheet.Row _config;
        private readonly IReadOnlyList<UnitStat> _stats;
        
        public StuffItem Item { get; protected set; }
        public StuffSlotType Type { get; private set; }
        public string BranchId => _config.BranchId;

        public StuffSlot(StuffData data, StuffSlotSheet.Row config, StuffSlotType type, IReadOnlyList<UnitStat> stats, IUnlockable unlockable) : base(data, unlockable)
        {
            _data = data;
            _config = config;
            Type = type;
            _stats = stats;
        }

        public void Equip(StuffItem item)
        {
            Item = item;
            base.Equip(item);
            
            foreach (var stat in _stats)
            {
                if (stat.Type != Item.EquippedEffectType) continue;
                Item.AddEquippedTarget(stat);
                break;
            }
        }

        public override void Remove()
        {
            _data.ItemId = string.Empty;
            foreach (var stat in _stats)
            {
                if (stat.Type != Item.EquippedEffectType) continue;
                Item.RemoveEquippedTarget(stat);
                break;
            }
            base.Remove();
        }
    }
}