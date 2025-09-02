using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.StatModifier
{
    [Serializable]
    public class StatModifierData
    {
        public HashSet<TeamType> TeamTypes = new();
        public HashSet<UnitSubType> IncludeTypes = new();
        public HashSet<UnitSubType> ExcludeTypes = new();
        
        public UnitStatType StatType;
        public StatModType ModType;
        
        [Preserve]
        public StatModifierData()
        {
        }
    }
}