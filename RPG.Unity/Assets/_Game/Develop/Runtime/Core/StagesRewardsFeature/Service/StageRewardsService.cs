using System;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Save;
using PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Sheet;
using PrimeTween;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Service
{
    public class StageRewardsService : IDisposable
    {
        [Inject] private MainMode _mainMode;
        [Inject] private StageRewardsDataProvider _dataProvider;
        [Inject] private ResourceService _resources;
        [Inject] private BalanceContainer _balance;

        private StageRewardsData _data;
        private int _currentStageId;
        private bool _isLevelChanged;
        private StageRewardsSheet.Row _stageRewardConfig;
        private StageRewardsSheet.Elem _reward;
        private Tween _resourceFxTween;

        public int CurrentIndex => _data.CurrentIndex;
        public StageRewardsSheet.Row StageRewardConfig => _stageRewardConfig;
        public event Action<bool> OnStageRewardTriggered;
        public event Action OnLevelChanged;

        public void Initialize()
        {
            _data = _dataProvider.GetData();
            _currentStageId = _mainMode.Id;
            LoadStageRewardConfigById(_currentStageId);
            _mainMode.OnLevelChanged += HandleStageChanged;
            _mainMode.OnLevelEntered += CheckCondition;
        }

        public void Dispose()
        {
            _mainMode.OnLevelChanged -= HandleStageChanged;
            _mainMode.OnLevelEntered -= CheckCondition;
            _resourceFxTween.Stop();
        }

        private void CheckCondition()
        {
            if (_isLevelChanged)
            {
                if (_stageRewardConfig != null && _data.CurrentIndex < _stageRewardConfig.Count)
                    _reward = _stageRewardConfig[_data.CurrentIndex];
                else
                    _reward = null;
                
                _isLevelChanged = false;
                bool isFinalStage = _currentStageId != _mainMode.Id;
                OnStageRewardTriggered?.Invoke(isFinalStage);

                if (!isFinalStage)
                {
                    _data.CurrentIndex++;
                }
                else
                {
                    _data.CurrentIndex = 0;
                    _currentStageId = _mainMode.Id;
                    LoadStageRewardConfigById(_currentStageId);
                }
                OnLevelChanged?.Invoke();
            }
        }

        private void HandleStageChanged()
        {
            _isLevelChanged = true;
        }

        private void LoadStageRewardConfigById(int id)
        {
            _stageRewardConfig = _balance.Get<StageRewardsSheet>().FirstOrDefault(x => x.Id == id);
        }

        public void CollectReward(Vector3 position, float finalAnimationTime)
        {
            if (_reward == null)
                return;
            
            _resources.AddResource(_reward.RewardType, _reward.RewardAmount);
            var rewardType = _reward.RewardType;
            var rewardAmount = _reward.RewardAmount;
            _resourceFxTween.Stop();
            _resourceFxTween = Tween.Delay(finalAnimationTime, () => 
            { 
                _resources.SetFxRequest(ResourceFXRequest.Create(rewardType, rewardAmount, spawnPosition: position));
            });
            _reward = null;
        }
    }
}