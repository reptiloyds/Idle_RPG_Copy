using PleasantlyGames.RPG.Runtime.Core.ShopHub.Type;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.ShopHub.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ShopElementView : MonoBehaviour
    {
        [SerializeField] private ShopElement _type;
        
        public ShopElement Type => _type;

        public virtual void Show() => 
            gameObject.SetActive(true);

        public virtual void Hide() => 
            gameObject.SetActive(false);
    }
}