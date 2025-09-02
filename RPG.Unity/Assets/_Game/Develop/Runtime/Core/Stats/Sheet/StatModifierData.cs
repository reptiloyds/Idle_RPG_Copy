using System;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Stats.Sheet
{
    public class StatModifierData<T> where T : Enum
    {
        public T Type;
        public StatModType ModType;
        public float Value;

        [Preserve]
        public StatModifierData()
        {
            
        }
    }
}