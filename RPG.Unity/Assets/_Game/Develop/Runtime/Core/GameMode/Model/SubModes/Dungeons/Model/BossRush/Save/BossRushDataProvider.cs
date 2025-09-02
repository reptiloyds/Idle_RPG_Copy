using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Save;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Save
{
    [Serializable]
    public class BossRushDataContainer
    {
        public int AvailableLevel;
        public bool IsAutoSweep;
        public SubModeDataContainer SubModeData = new();
        
        [Preserve]
        public BossRushDataContainer()
        {
        }
    }
    
    public class BossRushDataProvider : BaseDataProvider<BossRushDataContainer>
    {
        [Preserve]
        public BossRushDataProvider()
        {
            
        }
        
        public override void LoadData()
        {
            base.LoadData();

            if (Data == null) 
                CreateData();
        }

        private void CreateData()
        {
            Data = new BossRushDataContainer()
            {
                AvailableLevel = 1,
            };
        }
    }
}