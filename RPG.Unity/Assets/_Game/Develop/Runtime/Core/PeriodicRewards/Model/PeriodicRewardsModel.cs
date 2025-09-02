using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Characters;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Items;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Resource;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Type;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Save;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Sheet;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Model
{
    public class PeriodicRewardsModel
    {
        [Inject] private TimeService _timeService;
        [Inject] private ITranslator _translator;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private ResourceService _resourceService;
        [Inject] private ItemFacade _itemFacade;
        [Inject] private CharacterService _characterService;
        
        private readonly PeriodicRewardSheet.Row _config;
        private readonly PeriodicRewardData _data;
        private readonly List<PeriodicReward> _rewards = new();

        private string _rewardNamePrefix;
        
        public PeriodicRewardVariant Variant => _data.Variant;
        public bool IsRewardReady { get; private set; }
        public string Name { get; private set; }
        public int RewardId => _data.RewardId;
        
        public PeriodicRewardsModel(PeriodicRewardSheet.Row config, PeriodicRewardData data)
        {
            _config = config;
            _data = data;
        }

        public void Initialize()
        {
            int daysFormLastReward = _timeService.DaysFrom(_data.LastRewardTime);
            CreateRewards();

            if (RewardId >= _rewards.Count)
            {
                if (_config.ResetOnComplete)
                    ResetProgress();
                else
                    IsRewardReady = false;
            }
            else if (_config.ResetProgressPeriod > 0 && daysFormLastReward > _config.ResetProgressPeriod) 
                ResetProgress();
            else if (daysFormLastReward >= _config.RewardPeriod)
                IsRewardReady = true;
            else
                IsRewardReady = false;

            Name = _translator.Translate(_config.NameToken);
            _rewardNamePrefix = _translator.Translate(TranslationConst.FullDay);
        }

        public bool IsRewardBig(int rewardId) => 
            _rewards.Count - 1 == rewardId;

        public IReadOnlyList<PeriodicReward> GetRewards() => 
            _rewards;

        public void ApplyReward(int rewardId, Vector3 viewWorldPosition)
        {
            if (rewardId >= _rewards.Count && rewardId < 0)
            {
                Debug.LogError($"Attempt to apply wrong reward by id {rewardId}");
                return;
            }
            
            _rewards[rewardId].Apply(viewWorldPosition);

            _data.LastRewardTime = _timeService.Now();
            _data.RewardId++;
            IsRewardReady = false;
        }

        public string GetRewardName(int rewardId)
        {
            var days = rewardId * _config.RewardPeriod + 1; 
            return $"{_rewardNamePrefix} {days}";
        }

        private void CreateRewards()
        {
            foreach (var rewardConfig in _config)
            {
                PeriodicReward reward = null;
                switch (rewardConfig.RewardType)
                {
                    case PeriodicRewardType.Resource:
                        reward = new ResourcePeriodicReward(rewardConfig, _spriteProvider, _translator, _resourceService);
                        break;
                    case PeriodicRewardType.Item:
                        reward = new ItemPeriodicReward(rewardConfig, _spriteProvider, _translator, _itemFacade);
                        break;
                    case PeriodicRewardType.Character:
                        reward = new CharacterPeriodicReward(rewardConfig, _spriteProvider, _translator, _characterService);
                        break;
                }
                reward!.Initialize();
                _rewards.Add(reward);
            }
        }

        private void ResetProgress()
        {
            IsRewardReady = true;
            _data.RewardId = 0;
        }
    }
}