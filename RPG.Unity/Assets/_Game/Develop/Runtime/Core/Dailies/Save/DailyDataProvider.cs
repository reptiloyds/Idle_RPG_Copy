using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Type;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Save
{
    [Serializable]
    public class DailyDataContainer
    {
        public readonly Dictionary<string, DailyData> DataDictionary = new();
        
        [UnityEngine.Scripting.Preserve]
        public DailyDataContainer()
        {
        }
    }

    [Serializable]
    public class DailyData
    {
        public int Progress;
        public bool Collected;
        
        [Preserve]
        public DailyData()
        {
        }
    }
    
    public class DailyDataProvider : BaseDataProvider<DailyDataContainer>
    {
        [Inject] private BalanceContainer _balance;
        
        [UnityEngine.Scripting.Preserve]
        public DailyDataProvider() { }
        
        public override void LoadData()
        {
            base.LoadData();
            if (Data == null) 
                CreateData();
            else
                ValidateData();
        }

        private void CreateData()
        {
            Data = new DailyDataContainer();
            var sheet = _balance.Get<DailiesSheet>();
            foreach (var row in sheet) 
                Data.DataDictionary.Add(row.Id, new DailyData());
        }

        private void ValidateData()
        {
            var sheet = _balance.Get<DailiesSheet>();
            foreach (var row in sheet)
            {
                if(Data.DataDictionary.ContainsKey(row.Id)) continue;
                Data.DataDictionary.Add(row.Id, new DailyData());
            }

            foreach (var kvp in Data.DataDictionary)
            {
                if(sheet.Contains(kvp.Key)) continue;
                Data.DataDictionary.Remove(kvp.Key);
            }
        }
    }
}