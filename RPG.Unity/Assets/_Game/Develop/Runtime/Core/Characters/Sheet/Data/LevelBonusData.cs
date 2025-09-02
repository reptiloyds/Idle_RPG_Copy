using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data
{
    [Serializable]
    public class LevelBonusData
    {
        public int[] Levels;
        
        [Preserve]
        public LevelBonusData()
        {
        }
    }
}