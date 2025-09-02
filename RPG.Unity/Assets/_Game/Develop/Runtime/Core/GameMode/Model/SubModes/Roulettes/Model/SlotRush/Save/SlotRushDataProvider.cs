using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Save;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.Save
{
    [Serializable]
    public class SlotDataContainer
    {
        public SubModeDataContainer SubModeData = new();
        
        [Preserve]
        public SlotDataContainer()
        {
        }
    }
    
    public class SlotRushDataProvider : BaseDataProvider<SlotDataContainer>
    {
        [Preserve]
        public SlotRushDataProvider()
        {
        }
        
        public override void LoadData()
        {
            base.LoadData();

            if (Data == null)
                Data = new SlotDataContainer();
        }
    }
}