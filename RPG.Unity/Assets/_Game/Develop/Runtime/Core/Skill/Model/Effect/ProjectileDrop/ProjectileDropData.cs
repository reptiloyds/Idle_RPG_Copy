using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Skill.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.ProjectileDrop
{
    [Serializable]
    public class ProjectileDropData
    {
        public HashSet<TeamType> TeamTypes = new();
        public SkillTargetType Target; 
        public float Damage;
        public int Amount;
        public float Delay;
        public float Radius;
        public float Speed;
        public float OffsetX;
        public float OffsetY;
        public float OffsetZ;
        public float SecondViewOffsetY;
        
        [Preserve]
        public ProjectileDropData()
        {
            
        }
    }
}