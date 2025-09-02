using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Deal.Model
{
    public class ResourcePrice
    {
        public ResourceModel ResourceModel { get; private set; }
        public ResourceType Type => ResourceModel.Type;
        public Sprite Sprite => ResourceModel.Sprite;
        public bool IsEnough => ResourceModel.IsEnough(Amount);
        public BigDouble.Runtime.BigDouble TotalValue => ResourceModel.Value;
        public BigDouble.Runtime.BigDouble Amount { get; private set; }
        public bool IsEmpty { get; private set; }

        public void Setup(ResourceModel resourceModel, BigDouble.Runtime.BigDouble amount)
        {
            ResourceModel = resourceModel;
            Amount = amount;
            IsEmpty = false;
        }
        
        public void Clear()
        {
            ResourceModel = null;
            Amount = 0;
            IsEmpty = true;
        }
    }
}