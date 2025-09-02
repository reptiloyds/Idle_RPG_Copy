using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Projectile
{
    [Serializable]
    public class ProjectileData
    {
        public HashSet<TeamType> TeamTypes = new();
        public LaunchType LaunchType;
        public float ParabolaHeight;
        public TargetType TargetType;
        public ShootPointType ShootPoint; 
        public float Delay;
        public int Speed;
        public float Radius;
        
        [Preserve]
        public ProjectileData()
        {
        }
    }
}