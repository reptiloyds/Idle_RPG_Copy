using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.VIP.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class VipPurchaseRewardsView : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private VipPurchaseRewardView _prefab;

        public void Setup(IReadOnlyCollection<VipPurchaseReward> rewards)
        {
            foreach (var reward in rewards) 
                Instantiate(_prefab, _container).Setup(reward);
        }
    }
}