using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Sheet;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Type;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Save
{
    [Serializable]
    public class PeriodicRewardDataContainer
    {
        public List<PeriodicRewardData> List = new();
        
        [UnityEngine.Scripting.Preserve] public PeriodicRewardDataContainer() { }
    }

    [Serializable]
    public class PeriodicRewardData
    {
        public PeriodicRewardVariant Variant;
        public int RewardId;
        public DateTime LastRewardTime;
        
        [UnityEngine.Scripting.Preserve] public PeriodicRewardData() { }
    }
    
    public class PeriodicRewardDataProvider : BaseDataProvider<PeriodicRewardDataContainer>
    {
        [Inject] private PeriodicRewardService _service;
        [Inject] private BalanceContainer _balanceContainer;
        
        [UnityEngine.Scripting.Preserve]
        public PeriodicRewardDataProvider() { }

        public override void LoadData()
        {
            base.LoadData();
            
            if (Data == null)
                CreateData();
            else
                ValidateData();

            _service.Setup(Data!.List);
        }

        private void CreateData()
        {
            Data = new PeriodicRewardDataContainer();
            var sheet = _balanceContainer.Get<PeriodicRewardSheet>();
            foreach (var config in sheet) 
                AddDataElement(config);
        }

        private void ValidateData()
        {
            var sheet = _balanceContainer.Get<PeriodicRewardSheet>();
            foreach (var config in sheet)
            {
                if(HasDataWithVariant(config.Id)) continue;
                AddDataElement(config);
            }
        }

        private bool HasDataWithVariant(PeriodicRewardVariant variant)
        {
            foreach (var data in Data.List)
                if (data.Variant == variant) return true;
            
            return false;
        }

        private void AddDataElement(PeriodicRewardSheet.Row config)
        {
            Data.List.Add(new PeriodicRewardData()
            {
                Variant = config.Id,
                RewardId = 0,
            });
        }
    }
}