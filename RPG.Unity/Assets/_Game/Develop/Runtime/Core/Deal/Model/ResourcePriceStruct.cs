using System;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Deal.Model
{
    [Serializable]
    public struct ResourcePriceStruct
    {
        public ResourceType Type;
        public float M;
        public long E;

        [Preserve]
        public ResourcePriceStruct(ResourceType type, float m, long e)
        {
            Type = type;
            M = m;
            E = e;
        }

        public BigDouble.Runtime.BigDouble GetValue() => 
            new(M, E);
    }
}