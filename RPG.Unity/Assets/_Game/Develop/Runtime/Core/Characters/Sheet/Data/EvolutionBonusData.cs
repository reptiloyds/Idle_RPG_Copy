using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data
{
    [Serializable]
    public class EvolutionBonusData
    {
        public int[] Evolutions;
        
        [Preserve]
        public EvolutionBonusData()
        {
        }
    }
}