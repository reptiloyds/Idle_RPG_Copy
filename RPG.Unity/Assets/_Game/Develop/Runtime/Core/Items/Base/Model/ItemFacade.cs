using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using UnityEngine.Scripting;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.Model
{
    public sealed class ItemFacade
    {
        [Inject] private UnitStatService _unitStatService;
        [Inject] private IAudioService _audioService;
        
        private readonly Dictionary<string, Item> _itemDictionary = new();
        private readonly List<Item> _items = new();
        
        public IReadOnlyList<Item> Items => _items;
        public event Action<Item> OnEnhanced;
        
        [UnityEngine.Scripting.Preserve]
        public ItemFacade()
        {
            
        }
        
        public void Add(Item item)
        {
            if (!_itemDictionary.TryAdd(item.Id, item))
            {
                Logger.LogError($"Item with id {item.Id} already exists");
                return;
            }
            
            _items.Add(item);
        }
        
        public Item GetItem(string id)
        {
            _itemDictionary.TryGetValue(id, out var item);
            return item;
        }
        
        public void ApplyOwnedEffects(ItemType type)
        {
            foreach (var item in Items)
            {
                if (type != item.Type) continue;
                var stat = _unitStatService.GetPlayerStat(item.OwnedEffectType);
                item.AddOwnedTarget(stat);
            }
        }
        
        public void Enhance(string id)
        {
            var item = GetItem(id);
            if (item == null) return;
            item.Enhance();
            _audioService.CreateLocalSound(UI_Effect.UI_ItemUpgrade).Play();
            OnEnhanced?.Invoke(item);
        }

        public void AddAmount(string id, int amount = 1)
        {
            if(amount <= 0) return;
            var item = GetItem(id);
            item?.AddAmount(amount);
        }
    }
}