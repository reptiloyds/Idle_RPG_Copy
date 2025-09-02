using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.DefeatEnemy
{
    [Serializable]
    public class QDefeatEnemyData
    {
        public int Amount;
        
        [Preserve]
        public QDefeatEnemyData()
        {
            
        }
    }
}