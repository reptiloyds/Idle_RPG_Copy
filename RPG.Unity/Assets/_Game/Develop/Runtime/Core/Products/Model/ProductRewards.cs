using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Characters;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Items;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards.NoAds;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Resource;
using PleasantlyGames.RPG.Runtime.Core.Products.Save;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Model
{
    public class ProductRewards
    {
        [Inject] private ITranslator _translator;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private ResourceService _resourceService;
        [Inject] private CharacterService _characterService;
        [Inject] private BranchService _branchService;
        [Inject] private IAdService _adService;
        [Inject] private ItemFacade _itemFacade;
        [Inject] private ContentService _contentService;
        [Inject] private StuffInventory _stuffInventory;
        
        private readonly ProductRow _config;
        private readonly ProductData _data;

        private readonly List<ProductReward> _list = new();
        private readonly List<ProductReward> _bonusList = new();
        public IReadOnlyList<ProductReward> List => _list;
        public IReadOnlyList<ProductReward> BonusList => _bonusList;
        public CharacterProductReward Character { get; private set; }
        public bool HasBonus => !_data.BonusCollected && _bonusList.Count > 0;

        public ProductRewards(ProductRow config, ProductData data)
        {
            _config = config;
            _data = data;
        }

        public void Initialize() => 
            CreateRewards();

        public void ApplyRewards()
        {
            if (!_data.BonusCollected)
            {
                foreach (var reward in _bonusList) 
                    reward.Apply();
                _data.BonusCollected = true;
            }
            foreach (var reward in List) 
                reward.Apply();
        }

        public ProductReward GetMergeReward()
        {
            foreach (var reward in List)
                if(!string.IsNullOrEmpty(reward.MergeKey))
                    return reward;
            return null;
        }

        private void CreateRewards()
        {
            foreach (var itemConfig in _config)
            {
                ProductReward reward = null;
                switch (itemConfig.ItemType)
                {
                    case ProductItemType.Resource:
                        reward = CreateResourceReward(itemConfig);
                        break;
                    case ProductItemType.Character:
                        if (Character != null) continue;
                        Character = CreateCharacterReward(_config, itemConfig);
                        reward = Character;
                        break;
                    case ProductItemType.NoAds:
                        reward = CreateNoAdsReward(itemConfig);
                        break;
                    case ProductItemType.Item:
                        reward = CreateItemReward(itemConfig);
                        break;
                }

                if (reward == null) continue;
                if (reward.IsBonus) 
                    _bonusList.Add(reward);
                else
                    _list.Add(reward);
                reward.Initialize();
            }   
        }

        private ResourceProductReward CreateResourceReward(ProductElem config)
        {
            var resourceData = JsonConvert.DeserializeObject<ResourceProductRewardData>(config.ItemJSON);
            return new ResourceProductReward(_resourceService, resourceData,
                config.BackColor, config);
        }

        private CharacterProductReward CreateCharacterReward(ProductRow config, ProductElem itemConfig)
        {
            var characterData = JsonConvert.DeserializeObject<CharacterProductRewardData>(itemConfig.ItemJSON);
            var character = _characterService.GetCharacter(characterData.Id);
            var unlockable = new AnyUnlockable();
            foreach (var branchId in character.BranchIds) 
                unlockable.Add(_branchService.GetBranch(branchId));
            return new CharacterProductReward(_characterService, characterData, character, itemConfig.BackColor, itemConfig, unlockable);
        }

        private NoAdsProductReward CreateNoAdsReward(ProductElem config)
        {
            var noAdsData = JsonConvert.DeserializeObject<NoAdsProductRewardData>(config.ItemJSON);
            var sprite = _spriteProvider.GetSprite(Asset.MainAtlas, noAdsData.ImageName);
            return new NoAdsProductReward(_adService, noAdsData, sprite,
                _translator.Translate(noAdsData.NameToken), config.BackColor, config);
        }

        private ItemProductReward CreateItemReward(ProductElem config)
        {
            var itemData = JsonConvert.DeserializeObject<ItemProductRewardData>(config.ItemJSON);
            Action<Item, int> applyCallback = null;
            var allUnlockable = new AllUnlockable();
            switch (itemData.Type)
            {
                case ItemType.Stuff:
                    allUnlockable.Add(_contentService.GetById(ContentConst.Branches));
                    var stuffItem = _stuffInventory.GetItem(itemData.Id);
                    var anyUnlockable = new AnyUnlockable();
                    foreach (var slot in _stuffInventory.Slots)
                    {
                        if(slot.Type != stuffItem.SlotType) continue;
                        anyUnlockable.Add(_contentService.GetBranch(slot.BranchId));
                    }
                    allUnlockable.Add(anyUnlockable);
                    break;
                case ItemType.Companion:
                    allUnlockable.Add(_contentService.GetById(ContentConst.Companions));
                    break;
                case ItemType.Skill:
                    allUnlockable.Add(_contentService.GetById(ContentConst.Skills));
                    break;
            }

            var item = _itemFacade.GetItem(itemData.Id);
            return new ItemProductReward(_itemFacade, item, itemData, item.RarityColor, config, allUnlockable);
        }
    }
}