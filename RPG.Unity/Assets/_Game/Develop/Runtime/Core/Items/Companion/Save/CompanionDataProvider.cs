using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Save.Models;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Companion.Save
{
    [Serializable]
    public class CompanionDataContainer
    {
        public List<CompanionData> List = new();
        
        [UnityEngine.Scripting.Preserve]
        public CompanionDataContainer()
        {
        }
    }

    [Serializable]
    public class CompanionData : SlotData
    {
        public int SlotId;
        
        [UnityEngine.Scripting.Preserve]
        public CompanionData()
        {
        }
    }
    
    public class CompanionDataProvider : BaseDataProvider<CompanionDataContainer>
    {
        private const int COMPANION_SLOTS = 5;

        [UnityEngine.Scripting.Preserve]
        public CompanionDataProvider() { }
        
        public override void LoadData()
        {
            base.LoadData();
            
            if (Data == null)
                CreateData();
        }

        private void CreateData()
        {
            Data = new CompanionDataContainer();
            var slotAmount = COMPANION_SLOTS;
            
            for (var i = 0; i < slotAmount; i++)
                Data.List.Add(new CompanionData
                {
                    SlotId = i,
                    ItemId = string.Empty
                });
        }
    }
}