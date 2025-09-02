using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition.MainMode.Enter
{
    [Serializable]
    public class MainModeEnterConditionData
    {
        public int Id;
        public int Level;
        
        [Preserve]
        public MainModeEnterConditionData()
        {
        }
    }
}