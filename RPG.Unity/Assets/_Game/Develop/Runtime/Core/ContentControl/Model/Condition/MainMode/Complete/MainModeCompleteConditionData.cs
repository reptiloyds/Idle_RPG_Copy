using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition.MainMode.Complete
{
    [Serializable]
    public class MainModeCompleteConditionData
    {
        public int Id;
        public int Level;
        
        [Preserve]
        public MainModeCompleteConditionData()
        {
        }
    }
}