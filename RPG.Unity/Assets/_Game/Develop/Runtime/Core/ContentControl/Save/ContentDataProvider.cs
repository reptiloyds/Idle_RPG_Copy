using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Sheet;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Save
{
    [Serializable]
    public class ContentDataContainer
    {
        public List<ContentData> List = new();
        
        [UnityEngine.Scripting.Preserve]
        public ContentDataContainer()
        {
        }
    }

    [Serializable]
    public class ContentData
    {
        public string Id;
        public bool IsUnlocked;
        
        [Preserve]
        public ContentData()
        {
        }
    }
    
    public class ContentDataProvider : BaseDataProvider<ContentDataContainer>
    {
        [Inject] private BalanceContainer _balance;
        [Inject] private ContentService _service;

        [UnityEngine.Scripting.Preserve]
        public ContentDataProvider() { }
        
        public override void LoadData()
        {
            base.LoadData();
            
            if(Data == null)
                CreateData();
            else
                ValidateData();

            _service.Setup(Data);
        }

        private void CreateData()
        {
            var contentSheet = _balance.Get<ContentSheet>();
            Data = new ContentDataContainer();
            foreach (var row in contentSheet) 
                Data.List.Add(new ContentData(){Id = row.Id, IsUnlocked = false});
        }

        private void ValidateData()
        {
            var contentSheet = _balance.Get<ContentSheet>();
            foreach (var row in contentSheet)
            {
                if(HasContentData(row.Id)) continue;
                Data.List.Add(new ContentData {Id = row.Id, IsUnlocked = false});
            }
        }

        private bool HasContentData(string id)
        {
            foreach (var data in Data.List)
                if (data.Id == id) return true;

            return false;
        }
    }
}