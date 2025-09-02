using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Save
{
    [Serializable]
    public class BlessingDataContainer
    {
        public List<SwitchableBlessingData> List = new();
        public List<BlessingData> GlobalList = new();
        
        [UnityEngine.Scripting.Preserve]
        public BlessingDataContainer() { }
    }
    
    [Serializable]
    public class BlessingData
    {
        public string Id;
        public int Level;
        public int Progression;
        public DateTime LastActivationTime;

        [Preserve]
        public BlessingData(string id, int level)
        {
            Id = id;
            Level = level;
            Progression = 0;
        }
    }

    [Serializable]
    public class SwitchableBlessingData : BlessingData
    {
        public int ActivationAmount;
        public int FreeActivations;
        
        public SwitchableBlessingData(string id, int level) : base(id, level) => 
            ActivationAmount = 0;
    }
    
    public class BlessingDataProvider : BaseDataProvider<BlessingDataContainer>
    {
        [Inject] private BalanceContainer _balance;
        
        [UnityEngine.Scripting.Preserve]
        public BlessingDataProvider() { }

        public override void LoadData()
        {
            base.LoadData();

            if(Data == null)
                CreateData();
        }

        private void CreateData()
        {
            Data = new BlessingDataContainer();
            var sheet = _balance.Get<BlessingSheet>();
            foreach (var config in sheet) 
                Data.List.Add(new SwitchableBlessingData(config.Id, 1));
            
            var globalSheet = _balance.Get<GlobalBlessingSheet>();
            foreach (var config in globalSheet) 
                Data.GlobalList.Add(new BlessingData(config.Id, 1));
        }
    }
}