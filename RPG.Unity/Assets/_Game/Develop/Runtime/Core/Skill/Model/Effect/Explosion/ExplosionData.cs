using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Explosion
{
    [Serializable]
    public class ExplosionData
    {
        public HashSet<TeamType> TeamTypes = new();
        
        public float Amount = 1;
        public float Delay;
        public float Radius;
        
        [Preserve]
        public ExplosionData()
        {
            
        }
    }
}