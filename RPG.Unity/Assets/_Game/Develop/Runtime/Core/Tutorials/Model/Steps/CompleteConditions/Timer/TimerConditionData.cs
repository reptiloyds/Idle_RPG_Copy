using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Timer
{
    [Serializable]
    public class TimerConditionData
    {
        public float Duration;
        
        [Preserve]
        public TimerConditionData()
        {
        }
    }
}