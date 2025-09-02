using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.Save
{
    [Serializable]
    public class ResourceDataContainer
    {
        public List<ResourceData> Resources = new ();
        
        [UnityEngine.Scripting.Preserve]
        public ResourceDataContainer()
        {
        }
    }

    [Serializable]
    public class ResourceData
    {
        public ResourceType Type;
        public BigDouble.Runtime.BigDouble Value;
    }
    
    public class ResourceDataProvider : BaseDataProvider<ResourceDataContainer>
    {
        [Inject] private BalanceContainer _balance;

        [UnityEngine.Scripting.Preserve]
        public ResourceDataProvider()
        { }

        public override void LoadData()
        {
            base.LoadData();
            
            if (Data != null)
                ValidateData(Data.Resources);
            else
                Data = CreateNewData();
        }

        private ResourceDataContainer CreateNewData()
        {
            var resourceDataList = new ResourceDataContainer();
            
            var resourceSheet = _balance.Get<ResourceSheet>();
            foreach (var row in resourceSheet)
                resourceDataList.Resources.Add(new ResourceData()
                {
                    Type = row.Id,
                    Value = row.Value,
                });
            
            return resourceDataList;
        }

        private void ValidateData(List<ResourceData> dataList)
        {
            var resourceSheet = _balance.Get<ResourceSheet>();
            foreach (var row in resourceSheet)
            {
                if (dataList.All(item => item.Type != row.Id)) 
                    dataList.Add(new ResourceData()
                    {
                        Type = row.Id,
                        Value = row.Value,
                    });
            }

            foreach (var data in dataList)
            {
                if (resourceSheet.Contains(data.Type)) continue;
                dataList.Remove(data);
            }
        }
    }
}