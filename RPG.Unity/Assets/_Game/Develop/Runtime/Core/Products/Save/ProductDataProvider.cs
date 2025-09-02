using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Model.Periodic;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Save
{
    [Serializable]
    public class ProductDataContainer
    {
        public Dictionary<PeriodicType, int> GuessOrders = new();
        public List<ProductData> List = new();
        
        [UnityEngine.Scripting.Preserve]
        public ProductDataContainer()
        {
        }
    }

    [Serializable]
    public class ProductData
    {
        public string Id;
        public int Purchases;
        public int LimitCounter;
        public bool BonusCollected;
        public DateTime LastPurchase;

        [Preserve]
        public ProductData()
        {
            
        }
    }
    
    public class ProductDataProvider : BaseDataProvider<ProductDataContainer>
    {
        [Inject] private BalanceContainer _balance;

        [UnityEngine.Scripting.Preserve]
        public ProductDataProvider() { }
        
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
            var sheet = _balance.Get<ProductsSheet>();
            Data = new ProductDataContainer();

            foreach (var value in Enum.GetValues(typeof(PeriodicType)))
            {
                var type = (PeriodicType)value;
                if(type == PeriodicType.None) continue;
                Data.GuessOrders[type] = -1;
            }

            foreach (var row in sheet) 
                AddData(row);
        }

        private void ValidateData()
        {
            var sheet = _balance.Get<ProductsSheet>();
            
            foreach (var value in Enum.GetValues(typeof(PeriodicType)))
            {
                var type = (PeriodicType)value;
                if(type == PeriodicType.None) continue;
                Data.GuessOrders.TryAdd(type, -1);
            }
            
            foreach (var row in sheet)
            {
                if(HasWithId(row.Id)) continue;
                AddData(row);
            }
        }

        private void AddData(ProductRow config)
        {
            Data.List.Add(new ProductData()
            {
                Id = config.Id,
                Purchases = 0,
                LimitCounter = 0
            });
        }

        private bool HasWithId(string id)
        {
            foreach (var data in Data.List)
                if (string.Equals(data.Id, id)) return true;

            return false;
        }
    }
}