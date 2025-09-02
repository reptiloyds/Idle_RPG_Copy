using System;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Definition
{
    [Serializable]
    public class DailyRouletteDefinition
    {
        public int FreeSpinAmount;
        public int SpinAmount;
        public float Cooldown;
        
        [Preserve]
        public DailyRouletteDefinition()
        {
        }
    }
}