using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Definition;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model.Decorator;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Save;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model
{
    public sealed class Lootbox : IUnlockable, IDisposable
    {
        private readonly LootboxConfiguration _configuration;
        private readonly ISaveService _saveService;
        private readonly TimeService _timeService;
        
        private readonly LootboxRow _config;
        private readonly LootboxData _data;
        private readonly ItemFacade _facade;
        private readonly IUnlockable _unlockable;
        private readonly ILootboxDecorator _decorator;
        private LootboxElem _levelConfig;
        private readonly int _maxLevel;
        private readonly List<(ItemRarityType rarity, float weight)> _chances = new();

        private readonly List<Item> ResultBuffer = new(64);
        private readonly List<Item> ValidItemBuffer = new(32);
        private readonly List<Item> NewItemsBuffer = new(32);
        
        public ItemType ItemType { get; }
        public List<LootboxResourcePurchaseData> PurchaseDataList => _config.ResourcePurchases;
        public bool IsLevelMax => _data.Level == _maxLevel;
        public string Id => _data.Id;
        public int Level => _data.Level;
        public int Experience => _data.Experience;
        public int TargetExperience => _levelConfig.ExpToLevelUp;
        public BonusOpenHandler BonusOpenHandler { get; private set; }
        public bool HasBonusOpen { get; private set; }

        public bool IsUnlocked => _unlockable == null || _unlockable.IsUnlocked;
        public string Condition => _unlockable == null ? string.Empty : _unlockable.Condition;
        public event Action<IUnlockable> OnUnlocked;
        
        public List<Sprite> Sprites { get; }
        public Color Color => _config.Color;
        public IReadOnlyList<Item> NewItems => NewItemsBuffer;

        public event Action<Lootbox, IReadOnlyList<Item>> OnItemsApplied;

        public Lootbox(LootboxRow config, LootboxData data, ItemFacade facade, IUnlockable unlockable, List<Sprite> sprites,
            ItemType type, ILootboxDecorator decorator, LootboxConfiguration configuration, ISaveService saveService, TimeService time)
        {
            _configuration = configuration;
            _saveService = saveService;
            _timeService = time;
            ItemType = type;
            _config = config;
            _data = data;
            _facade = facade;
            _unlockable = unlockable;
            _decorator = decorator;
            Sprites = sprites;
            if (!IsUnlocked) 
                _unlockable.OnUnlocked += OnUnlockableUnlocked;

            _maxLevel = _config.Count;
            UpdateLevelConfig();
        }

        public void Dispose()
        {
            if (!IsUnlocked) 
                _unlockable.OnUnlocked -= OnUnlockableUnlocked;
            BonusOpenHandler?.Dispose();
        }
        
        private void OnUnlockableUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlockableUnlocked;
            OnUnlocked?.Invoke(this);
        }

        public void Initialize()
        {
            HasBonusOpen = _config.BonusOpen != null;
            if (!HasBonusOpen) return;
            BonusOpenHandler = new BonusOpenHandler(_data, _config.BonusOpen, _timeService);
            BonusOpenHandler.Initialize();
            BonusOpenHandler.OnProcessed += OnBonusProcessed;
        }

        private void OnBonusProcessed(int rewardAmount) => 
            Open(rewardAmount);

        public void Open(int amount)
        {
            PrepareLoot(amount);
            ApplyLoot();
        }

        private void PrepareLoot(int amount)
        {
            var ftue = _configuration.GetFTUE(_data.Id, _data.OpenCounter);
            int ftueId = 0;
            for (var i = 0; i < amount; i++)
            {
                if (ftue != null && ftueId < ftue.ItemIds.Count)
                {
                    var item = _facade.GetItem(ftue.ItemIds[ftueId]);
                    ResultBuffer.Add(item);
                    ftueId++;
                    continue;
                }
                
                foreach (var chancePair in _levelConfig.Chances)
                    if (IsGroupAvailable(chancePair.Key)) 
                        _chances.Add((chancePair.Key, chancePair.Value));
                
                float sum = 0;
                float maxValue = 0;
                foreach (var chanceTuple in _chances) 
                    maxValue += chanceTuple.weight;
                var random = Random.Range(0, maxValue);
                
                foreach (var chanceTuple in _chances)
                {
                    sum += chanceTuple.weight;
                    if (sum < random) continue;
                    
                    FillValidItems(chanceTuple.rarity);
                    Item result = null;
                    if (ValidItemBuffer.Count > 0) 
                        result = ValidItemBuffer.GetRandomElement();
                    ValidItemBuffer.Clear();
                    if(result != null)
                        ResultBuffer.Add(result);
                    break;
                }
                
                _chances.Clear();
            }
            
            _data.OpenCounter++;
            if (!IsLevelMax) 
                AppendExperience(amount);

            NewItemsBuffer.Clear();
            
            foreach (Item item in ResultBuffer)
            {
                if (!item.IsUnlocked)
                    NewItemsBuffer.Add(item);
            }
        }

        private void FillValidItems(ItemRarityType rarityType)
        {
            foreach (var item in _facade.Items)
            {
                if(!IsItemAvailable(item, rarityType)) continue;
                ValidItemBuffer.Add(item);
            }
        }

        private bool IsGroupAvailable(ItemRarityType rarityType)
        {
            foreach (var item in _facade.Items)
            {
                if(!IsItemAvailable(item, rarityType)) continue;
                return true;
            }

            return false;
        }

        private bool IsItemAvailable(Item item, ItemRarityType rarityType)
        {
            if(item.Type != ItemType) return false;
            if(item.Rarity != rarityType) return false;
            if(item.IsLevelMax) return false;
            return _decorator == null || _decorator.IsItemAvailable(item);
        }

        private void ApplyLoot()
        {
            foreach (var item in ResultBuffer) 
                _facade.AddAmount(item.Id, 1);
            OnItemsApplied?.Invoke(this, ResultBuffer);
            ResultBuffer.Clear();
            
            _saveService.SaveAndLoadToCloudAsync();
        }

        private void AppendExperience(int experience)
        {
            _data.Experience += experience;
            if (_data.Experience >= _levelConfig.ExpToLevelUp)
                LevelUp();
        }

        private void LevelUp()
        {
            _data.Experience -= _levelConfig.ExpToLevelUp;
            _data.Level++;
            UpdateLevelConfig();
        }

        private void UpdateLevelConfig()
        {
            if (_config.Count <= _data.Level)
            {
                Debug.LogError($"LootboxElem for {_data.Level} does not exist");
                return;
            }
            
            _levelConfig = _config[_data.Level - 1];
        }
    }
}