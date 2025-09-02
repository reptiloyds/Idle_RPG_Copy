using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.BuyItem
{
    [Preserve]
    public class DBuyItemData
    {
        public ItemType Type;
        public int Amount;
        
        [Preserve]
        public DBuyItemData() { }
    }
}