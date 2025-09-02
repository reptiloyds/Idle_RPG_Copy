using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Items.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Sheet;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.Save
{
    [Serializable]
    public class ItemDataContainer
    {
        public Dictionary<ItemType, List<ItemData>> Dictionary = new();
        
        [UnityEngine.Scripting.Preserve]
        public ItemDataContainer()
        {
            
        }
    }

    [Serializable]
    public class ItemData
    {
        public string Id;
        public int Level;
        public int Amount;
        public bool IsUnlocked;
        
        [UnityEngine.Scripting.Preserve]
        public ItemData()
        {
            
        }
    }
    
    public class ItemDataProvider : BaseDataProvider<ItemDataContainer>
    {
        [Inject] private BalanceContainer _balanceContainer;

        [UnityEngine.Scripting.Preserve]
        public ItemDataProvider() { }
        
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
            Data = new ItemDataContainer();
            CreateList(ItemType.Stuff, _balanceContainer.Get<StuffItemSheet>());
            CreateList(ItemType.Companion, _balanceContainer.Get<CompanionItemSheet>());
            CreateList(ItemType.Skill, _balanceContainer.Get<SkillItemSheet>());
        }

        private void CreateList(ItemType type, IEnumerable<ItemRow> itemRow)
        {
            var dataList = new List<ItemData>();
            foreach (var item in itemRow)
                dataList.Add(new ItemData {Id = item.Id, Level = 1, Amount = 0});
            Data.Dictionary[type] = dataList;
        }

        private void ValidateData()
        {
            ValidateList(ItemType.Stuff, _balanceContainer.Get<StuffItemSheet>());
            ValidateList(ItemType.Companion, _balanceContainer.Get<CompanionItemSheet>());
            ValidateList(ItemType.Skill, _balanceContainer.Get<SkillItemSheet>());
        }

        private void ValidateList(ItemType type, IEnumerable<ItemRow> itemRow)
        {
            var dataList = Data.Dictionary[type];
            foreach (var item in itemRow)
            {
                if (!HasItemWithId(dataList, item.Id))
                    dataList.Add(new ItemData {Id = item.Id, Level = 1, Amount = 0});
            }
        }

        private bool HasItemWithId(List<ItemData> items, string id)
        {
            foreach (var item in items)
                if(item.Id == id) return true;
            return false;
        }
    }
}