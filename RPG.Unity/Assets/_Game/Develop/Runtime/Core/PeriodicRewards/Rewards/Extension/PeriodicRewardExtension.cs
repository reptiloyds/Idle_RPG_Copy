using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Characters;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Items;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Resource;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Extension
{
    public static class PeriodicRewardExtension
    {
        public static void TryDeserialize(this PeriodicRewardType type, string json)
        {
            switch (type)
            {
                case PeriodicRewardType.Resource:
                    var resourceData = JsonConvertLog.DeserializeObject<ResourcePeriodicData>(json);
                    if (resourceData.ResourceType == ResourceType.None) 
                        Debug.LogError($"{typeof(ResourcePeriodicData)} has ResourceType = None");
                    if (resourceData.Amount <= 0) 
                        Debug.LogError($"{typeof(ResourcePeriodicData)} has Amount = {resourceData.Amount}");
                    break;
                case PeriodicRewardType.Item:
                    var itemData = JsonConvertLog.DeserializeObject<ItemPeriodicData>(json);
                    if (itemData.Id == null) 
                        Debug.LogError($"{typeof(ItemPeriodicData)} has ItemId = null");
                    if (itemData.Amount <= 0) 
                        Debug.LogError($"{typeof(ItemPeriodicData)} has Amount = {itemData.Amount}");
                    break;
                case PeriodicRewardType.Character:
                    var characterData = JsonConvertLog.DeserializeObject<CharacterPeriodicData>(json);
                    if (characterData.Id == null) 
                        Debug.LogError($"{typeof(CharacterPeriodicData)} has CharacterId = null");
                    break;
            }
        }
    }
}