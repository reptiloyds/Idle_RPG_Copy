using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Collections.Sheets;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Collections.Save
{
    [Serializable]
    public class CollectionDataContainer
    {
        public List<CollectionData> List = new();
        
        [UnityEngine.Scripting.Preserve]
        public CollectionDataContainer()
        {
        }
    }

    [Serializable]
    public class CollectionData
    {
        public string Id;
        public int Level;
        
        [UnityEngine.Scripting.Preserve]
        public CollectionData()
        {
        }
    }
    
    public class CollectionDataProvider : BaseDataProvider<CollectionDataContainer>
    {
        [Inject] private BalanceContainer _balance;
        
        [UnityEngine.Scripting.Preserve]
        public CollectionDataProvider()
        {
        }
        
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
            Data = new CollectionDataContainer();
            var sheet = _balance.Get<CollectionSheet>();
            foreach (var config in sheet) 
                AddData(config);
        }

        private void ValidateData()
        {
            var sheet = _balance.Get<CollectionSheet>();

            for (var i = 0; i < Data.List.Count; i++)
            {
                var data = Data.List[i];
                if (sheet.Contains(data.Id)) continue;
                Data.List.RemoveAt(i);
                i--;
            }
            
            foreach (var config in sheet)
            {
                if(!HasDataWithId(config.Id))
                    AddData(config);
            }
        }

        private void AddData(CollectionRow config)
        {
            Data.List.Add(new CollectionData()
            {
                Id = config.Id,
                Level = 0,
            });
        }

        private bool HasDataWithId(string id)
        {
            foreach (var data in Data.List)
                if (string.Equals(data.Id, id)) return true;

            return false;
        }
    }
}