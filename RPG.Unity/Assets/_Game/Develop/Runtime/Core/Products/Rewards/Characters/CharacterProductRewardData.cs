using System;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Characters
{
    [Serializable]
    public class CharacterProductRewardData
    {
        public string Id;
        public bool UseLastImage;
        
        [Preserve]
        public CharacterProductRewardData()
        {
            
        }
    }
}