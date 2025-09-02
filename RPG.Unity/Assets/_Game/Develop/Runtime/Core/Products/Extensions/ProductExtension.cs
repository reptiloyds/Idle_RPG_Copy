using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Characters;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Items;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards.NoAds;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Resource;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Extensions
{
    public static class ProductExtension
    {
        public static void TryDeserialize(this ProductItemType type, string json)
        {
            switch (type)
            {
                case ProductItemType.Resource:
                    var resourceData = JsonConvertLog.DeserializeObject<ResourceProductRewardData>(json);
                    if(resourceData.Type == ResourceType.None)
                        Debug.LogError("ResourceType should not be equals to None");
                    if(resourceData.Amount <= 0)
                        Debug.LogError("Amount should not be less or equal to 0");
                    break;
                case ProductItemType.Character:
                    var characterData = JsonConvertLog.DeserializeObject<CharacterProductRewardData>(json);
                    if (string.IsNullOrEmpty(characterData.Id)) 
                        Debug.LogError("Id should not be null or empty");
                    break;
                case ProductItemType.NoAds:
                    var noAdsData = JsonConvertLog.DeserializeObject<NoAdsProductRewardData>(json);
                    if (string.IsNullOrEmpty(noAdsData.ImageName))
                        Debug.LogError("ImageName should not be null or empty");
                    break;
                case ProductItemType.Item:
                    var itemData = JsonConvertLog.DeserializeObject<ItemProductRewardData>(json);
                    if (string.IsNullOrEmpty(itemData.Id))
                        Debug.LogError("ItemId should not be null or empty");
                    if (itemData.Amount <= 0)
                        Debug.LogError("Amount should not be less or equal to 0");
                    if (itemData.Type == ItemType.None)
                        Debug.LogError("ItemType should not be equals to None");
                    break;
            }
        }
    }
}