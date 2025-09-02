using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.CompleteLevel
{
    [Serializable]
    public class QCompleteLevelData
    {
        public int Id;
        public int Level;
        
        [Preserve]
        public QCompleteLevelData()
        {
        }
    }
}