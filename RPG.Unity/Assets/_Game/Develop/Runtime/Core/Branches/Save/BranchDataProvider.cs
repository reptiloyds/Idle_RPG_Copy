using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.Sheet;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Branches.Save
{
    [Serializable]
    public class BranchDataContainer
    {
        public string SelectedBranchId;
        public List<BranchData> List = new();

        [UnityEngine.Scripting.Preserve]
        public BranchDataContainer()
        {
        }
    }

    [Serializable]
    public class BranchData
    {
        public string Id;
        public string CharacterId;

        [UnityEngine.Scripting.Preserve]
        public BranchData(string id, string characterId)
        {
            Id = id;
            CharacterId = characterId;
        }
    }
    
    public class BranchDataProvider : BaseDataProvider<BranchDataContainer>
    {
        [Inject] private BalanceContainer _balance;

        [UnityEngine.Scripting.Preserve]
        public BranchDataProvider() : base()
        {
        }
        
        public override void LoadData()
        {
            base.LoadData();

            if (Data == null)
            {
                Data = new BranchDataContainer();
                FillStartData();
            }
            else
                ValidateData();
        }

        private void FillStartData()
        {
            var branchSheet = _balance.Get<BranchSheet>();
            foreach (var row in branchSheet) 
                Data.List.Add(new BranchData(row.Id, row.CharacterId));
            Data.SelectedBranchId = branchSheet[0].Id;
        }

        private void ValidateData()
        {
            var branchSheet = _balance.Get<BranchSheet>();
            foreach (var row in branchSheet)
            {
                if(!HasBranchWithId(row.Id))
                    Data.List.Add(new BranchData(row.Id, row.CharacterId));
            }
        }

        private bool HasBranchWithId(string id)
        {
            foreach (var branchData in Data.List)
                if (branchData.Id == id) return true;

            return false;
        }
    }
}