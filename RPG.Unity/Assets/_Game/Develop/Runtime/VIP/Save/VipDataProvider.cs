using System;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.VIP.Save
{
    [Serializable]
    public class VipDataContainer
    {
        public DateTime ActivationDate;
        public int ActivationAmount;
        public bool ShouldOfferExtension;
        
        [Preserve]
        public VipDataContainer()
        {
        }
    }
    
    public class VipDataProvider : BaseDataProvider<VipDataContainer>
    {
        [Preserve]
        public VipDataProvider() { }

        public override void LoadData()
        {
            base.LoadData();

            Data ??= CreateData();
        }

        private VipDataContainer CreateData() => new();
    }
}