using System;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Resource
{
    [Serializable]
    public class ResourcePeriodicData
    {
        public ResourceType ResourceType;
        public int Amount;
        
        [Preserve] public ResourcePeriodicData() { }
    }
}