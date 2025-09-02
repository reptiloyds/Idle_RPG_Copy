using System;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Save
{
    [Serializable]
    public class DailyRouletteData
    {
        public int SpinAmount;
        public int FreeSpinAmount;
        public DateTime LastSpinTime;
        
        [UnityEngine.Scripting.Preserve]
        public DailyRouletteData()
        {
        }
    }
    
    public class DailyRouletteDataProvider : BaseDataProvider<DailyRouletteData>
    {
        [Inject] private DailyRoulette _dailyRoulette;
        [Inject] private BalanceContainer _balance;
        
        [UnityEngine.Scripting.Preserve]
        public DailyRouletteDataProvider() { }
        
        public override void LoadData()
        {
            base.LoadData();

            if (Data == null) 
                CreateData();
            
            _dailyRoulette.Setup(Data);
        }

        private void CreateData() => 
            Data = new DailyRouletteData();
    }
}