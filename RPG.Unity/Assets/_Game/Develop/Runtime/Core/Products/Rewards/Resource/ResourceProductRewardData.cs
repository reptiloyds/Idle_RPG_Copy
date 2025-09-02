using System;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Resource
{
    [Serializable]
    public class ResourceProductRewardData
    {
        public ResourceType Type;
        public int Amount;

        [Preserve]
        public ResourceProductRewardData()
        {
            
        }
    }
}