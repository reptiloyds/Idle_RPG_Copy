using System;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.ReadOnly
{
    public interface IReadOnlySlot
    {
        bool IsUnlocked { get; }
        string UnlockCondition { get; }
        Item Item { get; }
        
        event Action OnUnlock;
        event Action OnEquip;
        event Action OnRemove;
    }
}