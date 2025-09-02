using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Serialization;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Save
{
    [Serializable]
    public class StuffDataContainer
    {
        public List<StuffData> List = new();
        
        [UnityEngine.Scripting.Preserve]
        public StuffDataContainer()
        {
        }
    }

    [Serializable]
    public class StuffData : SlotData
    {
        public string Id;
        
        [UnityEngine.Scripting.Preserve]
        public StuffData()
        {
            
        }
    }
    
    public class StuffDataProvider : BaseDataProvider<StuffDataContainer>
    {
        [Inject] private BalanceContainer _balanceContainer;

        [UnityEngine.Scripting.Preserve]
        public StuffDataProvider() { }
        
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
            var sheet = _balanceContainer.Get<StuffSlotSheet>();
            Data = new StuffDataContainer();
            foreach (var row in sheet) 
                CreateStuff(row);
        }

        private void ValidateData()
        {
            var sheet = _balanceContainer.Get<StuffSlotSheet>();
            foreach (var row in sheet)
            {
                if (!HasDataWithId(row.Id)) 
                    CreateStuff(row);
            }

            for (var i = 0; i < Data.List.Count; i++)
            {
                var data = Data.List[i];
                if (sheet.Contains(data.Id)) continue;
                Data.List.RemoveAt(i);
                i--;
            }
        }

        private bool HasDataWithId(string id)
        {
            foreach (var stuffData in Data.List)
                if (stuffData.Id == id)
                    return true;

            return false;
        }

        private void CreateStuff(StuffSlotSheet.Row row)
        {
            Data.List.Add(new StuffData
            {
                Id = row.Id,
                ItemId = string.Empty,
            });
        }
    }
}