using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Quest
{
    [Serializable]
    public class QuestTutorialData
    {
        public int QuestId;
        
        [Preserve]
        public QuestTutorialData()
        {
        }
    }
}