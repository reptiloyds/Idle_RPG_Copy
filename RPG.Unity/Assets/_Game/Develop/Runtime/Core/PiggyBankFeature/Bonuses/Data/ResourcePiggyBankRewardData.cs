using System;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Data
{
    [Serializable]
    public class ResourcePiggyBankRewardData
    {
        public ResourceType Type;
        public int Amount;

        [Preserve]
        public ResourcePiggyBankRewardData()
        {
            
        }
    }
}