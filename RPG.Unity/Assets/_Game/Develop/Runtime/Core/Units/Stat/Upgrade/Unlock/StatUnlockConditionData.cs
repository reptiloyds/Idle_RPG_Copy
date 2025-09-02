using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Unlock
{
    public enum StatUnlockType
    {
        None = 0,
        StatLevel = 1,
    }

    public class StatLevelUnlockData
    {
        public UnitStatType Type;
        public int Level;
        
        [Preserve]
        public StatLevelUnlockData()
        {
        }
    }
}