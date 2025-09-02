using System;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.AppendResource
{
    [Serializable]
    public class AppendResourceData
    {
        public ResourceType ResourceType;
        public int Amount;
        
        [Preserve]
        public AppendResourceData()
        {
        }
    }
}