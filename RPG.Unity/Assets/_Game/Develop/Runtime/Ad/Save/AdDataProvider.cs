using System;
using PleasantlyGames.RPG.Runtime.Save.Models;

namespace PleasantlyGames.RPG.Runtime.Ad.Save
{
    [Serializable]
    public class AdDataContainer
    {
        public bool IsDisabled;
        public int RewardAdViews;

        [UnityEngine.Scripting.Preserve]
        public AdDataContainer()
        {
        }
    }
    
    public class AdDataProvider : BaseDataProvider<AdDataContainer>
    {
        [UnityEngine.Scripting.Preserve]
        public AdDataProvider()
        {
        }

        public override void LoadData()
        {
            base.LoadData();

            if (Data == null) 
                CreateData();
        }

        private void CreateData() =>
            Data = new AdDataContainer
            {
                IsDisabled = false,
                RewardAdViews = 0
            };
    }
}