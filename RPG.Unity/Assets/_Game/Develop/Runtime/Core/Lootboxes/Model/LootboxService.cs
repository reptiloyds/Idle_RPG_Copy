using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Definition;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model.Decorator;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model.Stuff;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Save;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model
{
    public class LootboxService : IDisposable
    {
        private LootboxDataContainer _data;
        private readonly List<Lootbox> _lootboxes = new();

        [Inject] private BalanceContainer _balance;
        [Inject] private ContentService _contentService;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private LootboxDataProvider _dataProvider;
        [Inject] private LootboxConfiguration _configuration;
        [Inject] private ISaveService _saveService;
        [Inject] private TimeService _timeService;
        [Inject] private ItemFacade _itemFacade;
        [Inject] private StuffInventory _stuffInventory;
        [Inject] private BranchService _branchService;
        [Inject] private IAnalyticsService _analytics;

        public IReadOnlyDictionary<ItemType, int> PurchaseStatistic => _data.PurchaseStatistic;
        public event Action<Lootbox, IReadOnlyList<Item>> OnItemsApplied;
        public IReadOnlyList<Lootbox> Lootboxes => _lootboxes;

        [Preserve]
        public LootboxService()
        {
        }

        public void Initialize() => 
            CreateModels();

        private void CreateModels()
        {
            _data = _dataProvider.GetData();
            var sheet = _balance.Get<LootboxSheet>();
            
            foreach (var data in _data.DataList)
            {
                var config = sheet[data.Id];
                if(config == null) continue;
                var lootbox = CreateLootbox(config, data);
                _lootboxes.Add(lootbox);
            }
        }

        private Lootbox CreateLootbox(LootboxRow config, LootboxData data)
        {
            var content = _contentService.GetLootbox(data.Id);
            List<Sprite> sprites = null;
            ILootboxDecorator decorator = null;
            switch (config.ItemType)
            {
                case ItemType.Stuff:
                    sprites = _spriteProvider.GetSprites(Asset.StuffAtlas, config.Sprites);
                    decorator = new LootboxStuffDecorator(_stuffInventory, _branchService);
                    break;
                case ItemType.Companion:
                    sprites = _spriteProvider.GetSprites(Asset.CompanionAtlas, config.Sprites);
                    break;
                case ItemType.Skill:
                    sprites = _spriteProvider.GetSprites(Asset.SkillAtlas, config.Sprites);
                    break;
                default:
                    Debug.LogError($"Lootbox type of {config.ItemType} is not defined");
                    return null;
            }

            var lootbox = new Lootbox(config, data, _itemFacade, content,
                sprites, config.ItemType, decorator, _configuration, _saveService, _timeService);
            lootbox.Initialize();
            lootbox.OnItemsApplied += OnLootboxItemsApplied;
            
            return lootbox;
        }

        private void OnLootboxItemsApplied(Lootbox lootbox, IReadOnlyList<Item> items)
        {
            _data.PurchaseStatistic.TryAdd(lootbox.ItemType, 0);
            _data.PurchaseStatistic[lootbox.ItemType] += items.Count;
            OnItemsApplied?.Invoke(lootbox, items);
        }

        public void Purchase(Lootbox model, int amount)
        {
            // TODO ADD PURCHASE LOGIC
            _analytics.SendLootboxBought(model.ItemType, amount);
            model.Open(amount);
        }

        void IDisposable.Dispose()
        {
            foreach (var lootbox in _lootboxes)
            {
                lootbox.OnItemsApplied -= OnLootboxItemsApplied;
                lootbox.Dispose();
            }
        }
    }
}