using ProductType = PleasantlyGames.RPG.Runtime.InAppPurchase.Type.ProductType;

namespace PleasantlyGames.RPG.AndroidRuntime.Extensions
{
    public static partial class InAppExtension
    {
        public static UnityEngine.Purchasing.ProductType ConvertToUnityType(this ProductType productType)
        {
            switch (productType)
            {
                case ProductType.Consumable:
                    return UnityEngine.Purchasing.ProductType.Consumable;
                case ProductType.NonConsumable:
                    return UnityEngine.Purchasing.ProductType.NonConsumable;
                case ProductType.Subscription:
                    return UnityEngine.Purchasing.ProductType.Subscription;
                default:
                    return UnityEngine.Purchasing.ProductType.Consumable;
            }
        }
    }
}