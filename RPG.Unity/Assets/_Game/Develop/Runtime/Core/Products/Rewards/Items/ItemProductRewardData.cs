using System;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Items
{
    [Serializable]
    public class ItemProductRewardData
    {
        public ItemType Type;
        public string Id;
        public int Amount;
        
        [Preserve]
        public ItemProductRewardData()
        {
        }
    }
}