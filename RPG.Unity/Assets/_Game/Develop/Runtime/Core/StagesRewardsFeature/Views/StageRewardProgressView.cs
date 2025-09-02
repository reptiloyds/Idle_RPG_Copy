using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Service;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Views
{
    public class StageRewardProgressView : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Slider _progressSlider;
        [SerializeField, ReadOnly] private List<StageRewardView> _rewardViews = new List<StageRewardView>();

        [Inject] private StageRewardsService _service;
        [Inject] private ResourceService _resources;
        
        private Tween _activeTween;
        private Tween _resetTween;

        private void OnValidate()
        {
            Cache();
        }

        [Button]
        private void Cache()
        {
            _progressSlider = GetComponentInChildren<Slider>();
            _rewardViews = GetComponentsInChildren<StageRewardView>().ToList();
        }

        private void Start()
        {
            RedrawOnStart();
            _service.OnStageRewardTriggered += Redraw;
        }

        private void OnDestroy()
        {
            _service.OnStageRewardTriggered -= Redraw;
            _activeTween.Stop();
            _resetTween.Stop();
        }

        private void RedrawOnStart()
        {
            int currentIndex = _service.CurrentIndex;

            for (int i = 0; i < _rewardViews.Count; i++)
            {
                if (i < currentIndex)
                    _rewardViews[i].EnableCompleteImage();
                else
                    _rewardViews[i].DisableCompleteImage();
            }

            ConfigureViewsIcons();
            int filledIndex = Mathf.Clamp(currentIndex - 1, 0, _rewardViews.Count - 1);
            float targetValue = (float)filledIndex / (_rewardViews.Count - 1);
            _progressSlider.value = targetValue;
        }

        private void ConfigureViewsIcons()
        {
            for (int i = 0; i < _rewardViews.Count; i++)
            {
                if (i >= _service.StageRewardConfig.Count)
                    continue;

                var rewardType = _service.StageRewardConfig[i].RewardType;
                Sprite icon = _resources.GetResource(rewardType).Sprite;
                _rewardViews[i].Setup(icon);
            }
        }

        private void Redraw(bool isFinalStage)
        {
            int currentIndex = _service.CurrentIndex;

            if (_activeTween.isAlive || currentIndex >= _rewardViews.Count)
                return;

            float targetValue = (float)currentIndex / (_rewardViews.Count - 1);
            float finalAnimationTime = _rewardViews[currentIndex].AnimationDuration + 0.5f;
            _service.CollectReward(_rewardViews[currentIndex].transform.position, finalAnimationTime);
            
            _activeTween = Tween.Custom(
                startValue: _progressSlider.value,
                onValueChange: v => _progressSlider.value = v,
                endValue: targetValue,
                duration: 0.5f,
                ease: Ease.Default
            ).OnComplete(() =>
            {
                _rewardViews[currentIndex].PlayCompleteEffect();
                
                if (isFinalStage)
                {
                    _resetTween.Stop();
                    _resetTween = Tween.Delay(_rewardViews[currentIndex].AnimationDuration, Reset);
                }
            });
        }
        
        private void Reset()
        {
            _progressSlider.value = 0;

            foreach (StageRewardView view in _rewardViews)
                view.DisableCompleteImage();
            
            ConfigureViewsIcons();
        }
    }
}