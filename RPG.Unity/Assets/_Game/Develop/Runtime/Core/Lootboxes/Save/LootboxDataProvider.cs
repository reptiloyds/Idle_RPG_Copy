using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Serialization;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Save
{
    [Serializable]
    public class LootboxDataContainer
    {
        public readonly List<LootboxData> DataList = new(16);
        public readonly Dictionary<ItemType, int> PurchaseStatistic = new();
        
        [UnityEngine.Scripting.Preserve]
        public LootboxDataContainer()
        {
        }
    }

    [Serializable]
    public class LootboxData
    {
        public string Id;
        public int Level;
        public int Experience;
        public int OpenCounter;
        
        public int BonusOpenAmount;
        public DateTime LastBonusTime;
        public int BonusOpenCounter;
        
        [UnityEngine.Scripting.Preserve]
        public LootboxData()
        {
        }
    }
    
    public class LootboxDataProvider : BaseDataProvider<LootboxDataContainer>
    {
        [Inject] private BalanceContainer _balance;

        [UnityEngine.Scripting.Preserve]
        public LootboxDataProvider() { }
        
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
            Data = new LootboxDataContainer();
            var sheet = _balance.Get<LootboxSheet>();
            foreach (var row in sheet) 
                AppendData(row);
        }

        private void ValidateData()
        {
            var sheet = _balance.Get<LootboxSheet>();
            foreach (var row in sheet)
            {
                if(!HasDataWithId(row.Id))
                    AppendData(row);
            }
        }

        private bool HasDataWithId(string id)
        {
            foreach (var data in Data.DataList)
                if (string.Equals(data.Id, id)) return true;

            return false;
        }

        private void AppendData(LootboxRow row)
        {
            Data.DataList.Add(new LootboxData()
            {
                Id = row.Id,
                Level = 1,
                Experience = 0,
                OpenCounter = 0,
                LastBonusTime = default,
                BonusOpenAmount = 0,
                BonusOpenCounter = 0
            });
        }
    }
}