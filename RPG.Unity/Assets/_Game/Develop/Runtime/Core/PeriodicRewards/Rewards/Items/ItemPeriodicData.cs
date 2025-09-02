using System;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Items
{
    [Serializable]
    public class ItemPeriodicData
    {
        public string Id;
        public int Amount;
        
        [Preserve]
        public ItemPeriodicData() { }
    }
}