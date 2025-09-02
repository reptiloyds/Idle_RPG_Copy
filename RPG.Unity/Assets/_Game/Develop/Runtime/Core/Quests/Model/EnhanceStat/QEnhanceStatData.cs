using System;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.EnhanceStat
{
    [Serializable]
    public class QEnhanceStatData
    {
        public UnitStatType Type;
        public int Amount;
        
        [Preserve]
        public QEnhanceStatData()
        {
        }
    }
}