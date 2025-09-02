using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.Defenition
{
    [Serializable]
    public class ResourceConfiguration
    {
        public List<ResourceType> PresentInGame;
        public List<ResourceType> OfferInApp;

        public bool ShouldOfferInApp(ResourceType type)
        {
            foreach (var offerInApp in OfferInApp)
                if(offerInApp == type) return true;
            
            return false;
        }
    }
}