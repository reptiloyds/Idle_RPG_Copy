using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement.Forecast;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.Squad;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Companion
{
    public class CompanionSquad : BaseSquad, IAssetUser
    {
        [Inject] private CompanionInventory _inventory;
        private IReadOnlyList<CompanionItem> _items;
        
        private readonly Dictionary<CompanionItem, List<UnitStat>> _statsDictionary = new();
        private readonly List<(UnitView unit, CompanionItem item)> _unitTuples = new();

        public override TeamType TeamType => TeamType.Ally;
        
        [Preserve]
        public CompanionSquad()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _items = _inventory.Items;
            foreach (var item in _items)
            {
                item.OnLevelUp += OnItemLevelUp;
                var stats = StatService.CreateRuntimeStats(item.UnitId);
                _statsDictionary.Add(item, stats);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var item in _items) 
                item.OnLevelUp -= OnItemLevelUp;   
        }

        private void OnItemLevelUp(Item item)
        {
            var companionItem = _inventory.GetItem(item.Id);
            SetStatsLevel(GetStats(companionItem), item.Level);
        }

        public async UniTask SpawnUnitsAsync()
        {
            await UniTask.WhenAll(_inventory.Slots.Where(slot => slot.BaseItem != null)
                .Select(slot => SpawnAsync(slot.Item, slot.Id))
                .ToArray());

            _inventory.OnEquipped += OnItemEquipped;
            _inventory.OnRemoved += OnItemRemoved;
        }

        public override void Clear()
        {
            _inventory.OnEquipped -= OnItemEquipped;
            _inventory.OnRemoved -= OnItemRemoved;
            base.Clear();
        }

        private async void OnItemEquipped(CompanionSlot slot, CompanionItem item) => 
            await SpawnAsync(item, slot.Id);

        private void OnItemRemoved(CompanionSlot slot, CompanionItem item)
        {
            var unit = GetUnitByItem(item);
            if(unit != null)
                RemoveUnit(unit);
        }

        private async UniTask SpawnAsync(CompanionItem item, int spawnId)
        {
            var spawnPoint = SpawnProvider.GetSpawnPoint(spawnId);
            
            var unit = await SpawnUnitAsync(item.UnitId, spawnPoint);
            var stats = GetStats(item);
            unit.SetStats(stats);
            unit.Initialize();
            
            _unitTuples.Add((unit, item));

            SetStatsLevel(stats, item.Level);
            AppendUnit(unit);
        }

        protected override void RemoveUnit(UnitView unitView)
        {
            base.RemoveUnit(unitView);

            foreach (var tuple in _unitTuples)
            {
                if (tuple.unit != unitView) continue;
                _unitTuples.Remove(tuple);
                break;
            }
        }

        private void SetStatsLevel(List<UnitStat> stats, int level)
        {
            if(level <= 1) return;
            foreach (var stat in stats) 
                stat.SetLevel(level);  
        }

        private UnitView GetUnitByItem(CompanionItem item)
        {
            foreach (var tuple in _unitTuples)
            {
                if (tuple.item != item) continue;
                return tuple.unit;
            }

            return null;
        }

        public List<UnitStat> GetStats(CompanionItem item)
        {
            if (_statsDictionary.TryGetValue(item, out var stats)) return stats;
            
            stats = StatService.CreateRuntimeStats(item.UnitId);
            _statsDictionary.Add(item, stats);

            return stats;
        }

        public event Action OnNeedsChanged;
        
        public void FillNeeds(in Dictionary<AssetType, HashSet<string>> needs)
        {
            var unitSheet = Balance.Get<UnitsSheet>();
            var unitsHashSet = needs[AssetType.Unit];
            foreach (var slot in _inventory.Slots)
            {
                if(slot.BaseItem == null) continue;
                unitsHashSet.Add(unitSheet[slot.Item.UnitId].GetPrefabId(0));
            }
        }
    }
}