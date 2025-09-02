using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Save
{
    [Serializable]
    public class StatImproverDataContainer
    {
        public Dictionary<UnitStatType, int> ImproveStatistic = new();

        [Preserve]
        public StatImproverDataContainer()
        {
        }
    }
    
    public class StatImproverDataProvider : BaseDataProvider<StatImproverDataContainer>
    {
        [Preserve]
        public StatImproverDataProvider() { }

        public override void LoadData()
        {
            base.LoadData();

            Data ??= new StatImproverDataContainer();
        }
    }
}