using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.Model
{
    public abstract class BaseInventory<T> where T : Item
    {
        [Inject] private ItemFacade _itemFacade;
        [Inject] private ItemDataProvider _itemDataProvider;
        
        [Inject] protected BalanceContainer Balance;
        [Inject] protected ITranslator Translator;
        [Inject] protected ISpriteProvider SpriteProvider;
        [Inject] protected ItemConfiguration Configuration;

        [Inject] protected UnitStatService UnitStatService;

        private List<ItemData> _itemDataList;
        private readonly List<T> _items = new();
        private readonly Dictionary<string, T> _itemDictionary = new();

        public IReadOnlyList<T> Items => _items;

        protected abstract ItemType Type { get; }
        public event Action<T> OnEnhanced;

        [Preserve]
        protected BaseInventory()
        {
        }

        public virtual void Initialize()
        {
            _itemDataList = _itemDataProvider.GetData().Dictionary[Type];
            foreach (var itemData in _itemDataList)
            {
                var item = CreateItem(itemData);
                if(item == null) continue;
                if (!_itemDictionary.TryAdd(item.Id, item))
                {
                    Logger.LogError($"Item with id {item.Id} already exists");
                    return;
                }
                _items.Add(item);
                _itemFacade.Add(item);
            }

            ApplyOwnedEffects();
        }

        protected abstract T CreateItem(ItemData data);

        private void ApplyOwnedEffects() => 
            _itemFacade.ApplyOwnedEffects(Type);
        
        public T GetItem(string id)
        {
            _itemDictionary.TryGetValue(id, out var item);
            return item;
        }

        
        public T GetItem(ItemType type)
        {
            return _items.FirstOrDefault(x => x.Type == type);
        }

        public void Enhance(string id)
        {
            var item = GetItem(id);
            if (item == null) return;
            _itemFacade.Enhance(id);
            OnEnhanced?.Invoke(item);
        }
    }
}