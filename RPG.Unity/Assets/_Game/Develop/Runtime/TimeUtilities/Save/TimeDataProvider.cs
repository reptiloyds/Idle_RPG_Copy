using System;
using PleasantlyGames.RPG.Runtime.Save.Models;

namespace PleasantlyGames.RPG.Runtime.TimeUtilities.Save
{
    [Serializable]
    public class TimeDataContainer
    {
        public DateTime SaveTime;
        public float TotalPlaytime;
        
        [UnityEngine.Scripting.Preserve]
        public TimeDataContainer()
        {
        }
    }
    
    public class TimeDataProvider : BaseDataProvider<TimeDataContainer>
    {
        [UnityEngine.Scripting.Preserve]
        public TimeDataProvider() { }

        public override void LoadData()
        {
            base.LoadData();

            Data ??= new TimeDataContainer();
        }
    }
}