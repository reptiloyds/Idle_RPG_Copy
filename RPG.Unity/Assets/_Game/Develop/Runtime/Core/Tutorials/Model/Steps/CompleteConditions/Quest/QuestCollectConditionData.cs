using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Quest
{
    [Serializable]
    public class QuestCollectConditionData
    {
        public int QuestId;
        
        [Preserve]
        public QuestCollectConditionData()
        {
        }
    }
}