using System;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Service;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Views
{
    public class StageRewardIconView : MonoBehaviour
    {
        [SerializeField, Required] private Image _stageRewardImage;
        
        [Inject] private StageRewardsService _rewardsService;
        [Inject] private ResourceService _resources;
        
        private void Start()
        {
            SetupRewardIcon();
            _rewardsService.OnLevelChanged += SetupRewardIcon;
        }

        private void OnDestroy()
        {
            _rewardsService.OnLevelChanged -= SetupRewardIcon;
        }

        private void SetupRewardIcon()
        {
            int index = _rewardsService.CurrentIndex;

            if (_rewardsService.StageRewardConfig == null || index < 0 || index >= _rewardsService.StageRewardConfig.Count)
                return;

            var rewardType = _rewardsService.StageRewardConfig[index].RewardType;
            Sprite icon = _resources.GetResource(rewardType).Sprite;
            _stageRewardImage.sprite = icon;
        }
    }
}