using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.AreaDamage
{
    [Serializable]
    public class AreaDamageData
    {
        public HashSet<TeamType> TeamTypes = new();
        public float Delay;
        public float Radius;
        
        [Preserve]
        public AreaDamageData()
        {
        }
    }
}