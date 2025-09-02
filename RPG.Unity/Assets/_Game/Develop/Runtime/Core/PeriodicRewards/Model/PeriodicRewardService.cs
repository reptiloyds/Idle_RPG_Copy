using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Save;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Sheet;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Type;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Model
{
    public class PeriodicRewardService
    {
        [Inject] private BalanceContainer _balanceContainer;
        [Inject] private IObjectResolver _objectResolver;
        
        private List<PeriodicRewardData> _data;
        private readonly List<PeriodicRewardsModel> _models = new();

        [Preserve]
        public PeriodicRewardService() { }

        public void Setup(List<PeriodicRewardData> data) => 
            _data = data;
        
        public void Initialize() => 
            CreateModels();

        private void CreateModels()
        {
            var sheet = _balanceContainer.Get<PeriodicRewardSheet>();
            foreach (var data in _data)
            {
                var config = sheet[data.Variant];
                if (config == null) continue;
                var model = new PeriodicRewardsModel(config, data);
                _objectResolver.Inject(model);
                model.Initialize();
                _models.Add(model);
            }
        }

        public PeriodicRewardsModel GetModel(PeriodicRewardVariant variant)
        {
            foreach (var model in _models)
                if (model.Variant == variant) return model;
            return null;
        }

        public void ApplyReward(PeriodicRewardVariant variant, int rewardId, Vector3 viewWorldPosition)
        {
            var model = GetModel(variant);
            model.ApplyReward(rewardId, viewWorldPosition);
        }

        public bool IsRewardReady(PeriodicRewardVariant variant)
        {
            var model = GetModel(variant);
            if(model == null) return false;
            return model.IsRewardReady;
        }
    }
}