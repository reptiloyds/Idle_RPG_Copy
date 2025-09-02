using System;
using PleasantlyGames.RPG.Runtime.Save.Models;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Save
{
    [Serializable]
    public class PassiveIncomeDataContainer
    {
        public BigDouble.Runtime.BigDouble SoftIncome;
        public int TotalMinutes;
        public DateTime LastTickTime;
        
        [UnityEngine.Scripting.Preserve]
        public PassiveIncomeDataContainer()
        {
        }
    }
    
    public class PassiveIncomeDataProvider : BaseDataProvider<PassiveIncomeDataContainer>
    {
        [Inject] private TimeService _timeService;
        
        [UnityEngine.Scripting.Preserve]
        public PassiveIncomeDataProvider()
        {
           
        }
        
        public override void LoadData()
        {
            base.LoadData();

            if (Data == null)
                CreateData();
        }

        private void CreateData()
        {
            Data = new PassiveIncomeDataContainer()
            {
                SoftIncome = 0,
                TotalMinutes = 0,
                LastTickTime = _timeService.Now(),
            };
        }
    }
}