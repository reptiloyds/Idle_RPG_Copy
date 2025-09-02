using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Save;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.Save
{
    [Serializable]
    public class SoftRushDataContainer
    {
        public int AvailableLevel;
        public bool IsAutoSweep;
        public SubModeDataContainer SubModeData = new();

        [Preserve]
        public SoftRushDataContainer()
        {
        }
    }
    
    public class SoftRushDataProvider : BaseDataProvider<SoftRushDataContainer>
    {
        [Preserve]
        public SoftRushDataProvider()
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
            Data = new SoftRushDataContainer()
            {
                AvailableLevel = 1,
            };
        }
    }
}