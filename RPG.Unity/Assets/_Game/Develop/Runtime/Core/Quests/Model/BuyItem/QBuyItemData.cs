using System;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.BuyItem
{
    [Serializable]
    public class QBuyItemData
    {
        public ItemType Type;
        public int Amount;
        
        [Preserve]
        public QBuyItemData() { }
    }
}