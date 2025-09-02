using System;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet
{
    [Serializable]
    public class LootboxResourcePurchaseData
    {
        public ResourceType ResourceType;
        public int Price;
        public int ItemAmount;
        
        [Preserve]
        public LootboxResourcePurchaseData()
        {
        }
    }
    
    [Serializable]
    public class LootboxBonusOpenData
    {
        public int OpenAmount;
        public int Cooldown;
        public int StartAmount;
        public int EndAmount;
        
        [Preserve]
        public LootboxBonusOpenData()
        {
        }
    }
}