using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Data;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Extension
{
    public static class PiggyBankBonusesExtension
    {
        public static void TryDeserialize(this PiggyBankRewardType type, string json)
        {
            switch (type)
            {
                case PiggyBankRewardType.Resource:
                    var resourceData = JsonConvertLog.DeserializeObject<ResourcePiggyBankRewardData>(json);
                    if(resourceData.Type == ResourceType.None)
                        Debug.LogError("ResourceType should not be equals to None");
                    if(resourceData.Amount <= 0)
                        Debug.LogError("Amount should not be less or equal to 0");
                    break;
            }
        }
    }
}