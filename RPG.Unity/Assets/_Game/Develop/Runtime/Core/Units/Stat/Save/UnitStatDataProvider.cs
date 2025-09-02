using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Config;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Unlock;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Save
{
    [Serializable]
    public class UnitStatDataContainer
    {
        public List<StatData> Stats = new(); // BranchId -> List<StatData>

        [UnityEngine.Scripting.Preserve]
        public UnitStatDataContainer()
        {
        }
    }

    [Serializable]
    public class StatData
    {
        public UnitStatType Type;
        public int Level;
        public bool IsUnlocked;

        [UnityEngine.Scripting.Preserve]
        public StatData(UnitStatType type, int level, bool isUnlocked = true)
        {
            Type = type;
            Level = level;
            IsUnlocked = isUnlocked;
        }
    }

    public class UnitStatDataProvider : BaseDataProvider<UnitStatDataContainer>
    {
        [Inject] private BalanceContainer _balance;

        [UnityEngine.Scripting.Preserve]
        public UnitStatDataProvider()
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
            Data = new UnitStatDataContainer();
            var playerStatsSheet = _balance.Get<PlayerStatsSheet>();
            foreach (var row in playerStatsSheet)
                AddStat(row.Config);
        }

        private void AddStat(ImprovableUnitStatConfig config)
        {
            var isUnlocked = config.UnlockType == StatUnlockType.None;
            Data.Stats.Add(new StatData(config.StatType, 1, isUnlocked));
        }

        private void ValidateData()
        {
            var playerStatsSheet = _balance.Get<PlayerStatsSheet>();

            for (var i = Data.Stats.Count - 1; i >= 0; i--)
                if (!playerStatsSheet.Contains(Data.Stats[i].Type))
                    Data.Stats.RemoveAt(i);

            foreach (var row in playerStatsSheet)
                if (!ContainData(row.Config.StatType))
                    AddStat(row.Config);
        }

        private bool ContainData(UnitStatType type)
        {
            foreach (var data in Data.Stats)
                if (data.Type == type)
                    return true;
            return false;
        }
    }
}