using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Deal.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View.Evolution
{
    [DisallowMultipleComponent, HideMonoScript]
    public class EvolutionPrice : MonoBehaviour
    {
        [SerializeField] private List<EvolutionPriceElement> _priceElements;
        [Inject] private ResourceService _resourceService;

        public void Setup(List<ResourcePriceStruct> prices)
        {
            if (prices.Count > _priceElements.Count)
            {
                Debug.LogError("price.Count > uiPrice.Count");
                return;
            }

            for (int i = 0; i < _priceElements.Count; i++)
            {
                if (i >= prices.Count) 
                    _priceElements[i].Disable();
                else
                {
                    var price = prices[i];
                    var sprite = _resourceService.GetResource(price.Type).Sprite;
                    _priceElements[i].Setup(sprite, price.GetValue());
                    _priceElements[i].Enable();
                }
            }
        }
    }
}