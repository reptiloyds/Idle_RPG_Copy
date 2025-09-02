using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Bomber
{
    [Serializable]
    public class BomberData
    {
        public HashSet<TeamType> TeamTypes = new();
        public LaunchType LaunchType;
        public float ParabolaHeight;
        public TargetType TargetType;
        public float AttackRadius;
        public float ExplosionRadius;
        public float AttackSpeed;
        public float Height;
        public int Speed;
        public int MovementSpeed;
        
        [Preserve]
        public BomberData()
        {
        }
    }
}