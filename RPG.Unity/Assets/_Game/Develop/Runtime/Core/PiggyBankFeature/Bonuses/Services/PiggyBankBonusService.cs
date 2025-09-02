using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Data;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Model;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Save;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Sheets;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Services
{
    public class PiggyBankBonusService
    {
        [Inject] private PiggyBankBonusesDataProvider _dataProvider;
        [Inject] private BalanceContainer _balance;
        [Inject] private ResourceService _resource;
        [Inject] private ITranslator _translator;

        private PiggyBankBonusesDataContainer _data;
        private List<BasePiggyBankBonus> _bonuses = new List<BasePiggyBankBonus>();
        
        public IReadOnlyList<BasePiggyBankBonus> Bonuses => _bonuses;
        
        [Preserve]
        public PiggyBankBonusService()
        {
        }

        public void Initialize()
        {
            _data = _dataProvider.GetData();
            CreateBonuses();
        }
        
        private void CreateBonuses()
        {
            var sheet = _balance.Get<PiggyBankBonusesSheet>();
            
            foreach (var data in _data.Bonuses)
            {
                if (!sheet.TryGetValue(data.Id, out var config))
                    continue;

                _bonuses.Add(CreateBonus(data, config));
            }
        }
        
        private BasePiggyBankBonus CreateBonus(PiggyBankBonusData data, PiggyBankBonusesSheet.PiggyBankBonusRow config)
        {
            BasePiggyBankBonus productReward = null;
            
            switch (config.RewardType)
            {
                case PiggyBankRewardType.Resource:
                    productReward = CreateResourceReward(data, config);
                    break;
            }

            return productReward;
        }

        private BasePiggyBankBonus CreateResourceReward(PiggyBankBonusData data,
            PiggyBankBonusesSheet.PiggyBankBonusRow config)
        {
            var resourceData = JsonConvert.DeserializeObject<ResourcePiggyBankRewardData>(config.ItemJson);
            return new ResourcePiggyBankBonus(data, config, _translator, _resource, resourceData);
        }
    }
}