using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.ShootingObject
{
    [Serializable]
    public class ShootingObjectData
    {
        public HashSet<TeamType> TeamTypes = new();
        public LaunchType LaunchType;
        public float ParabolaHeight;
        public TargetType TargetType;
        public int Speed;
        public float Radius;

        [Preserve]
        public ShootingObjectData()
        {
            
        }
    }
}