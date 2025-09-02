using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.AssetManagement.Uploading.View;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Factory
{
    public class UnitFactory
    {
        private readonly Dictionary<string, ObjectPool<UnitView>> _poolDictionary = new();
        private readonly Dictionary<string, UnitView> _prefabs = new();
        private UnitsSheet _unitsSheet;
        private UnitsSheet UnitsSheet => _unitsSheet ??= _balance.Get<UnitsSheet>();

        private readonly List<UnitView> _activeUnits = new();

        [Inject] private IAssetProvider _assetProvider;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private UnitStatService _statService;
        [Inject] private BalanceContainer _balance;
        
        private const int DefaultCapacity = 3;

        public IReadOnlyList<UnitView> Units => _activeUnits;
        private readonly List<string> _keysToRemove = new(5);
        
        [Preserve]
        public UnitFactory() { }

        public async UniTask WarmUpAsync(HashSet<string> units)
        {
            foreach (var key in _prefabs.Keys)
                if (!units.Contains(key)) 
                    _keysToRemove.Add(key);

            foreach (var keyToRemove in _keysToRemove)
            {
                var pool = _poolDictionary[keyToRemove];
                if (pool.CountActive > 0) 
                    Logger.LogError($"Pool for {keyToRemove} is not empty");
                
                pool.Dispose();
                _poolDictionary.Remove(keyToRemove);
                _prefabs.Remove(keyToRemove);
                _assetProvider.Release(keyToRemove);
            }
            
            _keysToRemove.Clear();
            
            await UniTask.WhenAll(units.Select(WarmUpUnitAsync));
        }

        private async UniTask WarmUpUnitAsync(string unitKey) => 
            await LoadUnitAsync(unitKey, true);

        private async UniTask LoadUnitAsync(string unitKey, bool isWarmUp)
        {
            if (_prefabs.ContainsKey(unitKey)) return;
            var unitObject = await _assetProvider.LoadAssetAsync<GameObject>(unitKey, !isWarmUp);
            if (unitObject.TryGetComponent(out UnitView unit)) 
                _prefabs[unitKey] = unit;
        }
        
        public async UniTask<UnitView> CreateAsync(string unitId, int evolution, bool isDummy = false)
        {
            var unitConfig = UnitsSheet[unitId];
            var prefabId = unitConfig.GetPrefabId(evolution);
            
            var unit = await InstantiateUnit(prefabId);
            unit.SetId(unitId, prefabId);
            unit.SetSubTypes(unitConfig.TypeTags);
            
            if(!isDummy) 
                _activeUnits.Add(unit);
            
            return unit;
        }

        public void Release(UnitView unitView)
        {
            if (_poolDictionary.TryGetValue(unitView.PrefabId, out var pool))
            {
                unitView.transform.parent = null;
                _activeUnits.Remove(unitView);
                pool.Release(unitView);
            }
            else
                Debug.LogError("Unit pool not found");
        }

        private async UniTask<UnitView> InstantiateUnit(string prefabId)
        {
            UnitView unitView;
            if (_poolDictionary.TryGetValue(prefabId, out var pool))
                unitView = pool.Get();
            else
            {
                if (!_prefabs.ContainsKey(prefabId)) 
                    await LoadUnitAsync(prefabId, false); 
                var newPool = CreatePool(_prefabs[prefabId]);
                _poolDictionary[prefabId] = newPool;
                unitView = newPool.Get();
            }

            return unitView;
        }

        private ObjectPool<UnitView> CreatePool(UnitView prefab)
        {
            var pool = new ObjectPool<UnitView>(() => Instantiate(prefab), defaultCapacity: DefaultCapacity,
                actionOnRelease: ActionOnRelease, actionOnGet: ActionOnGet, actionOnDestroy: ActionOnDestroy);
            return pool;
            
            void ActionOnRelease(UnitView unitView)
            {
                unitView.Dispose();
                unitView.gameObject.SetActive(false);
            }
            
            void ActionOnGet(UnitView unitView) => 
                unitView.gameObject.SetActive(true);
            
            void ActionOnDestroy(UnitView unitView) => 
                Object.Destroy(unitView.gameObject);
            
            UnitView Instantiate(UnitView template)
            {
                var instance = Object.Instantiate(template);
                _objectResolver.InjectGameObject(instance.gameObject);
                return instance;
            }
        }

        public UnitView Search(TeamType teamType, string unitId)
        {
            foreach (var unit in _activeUnits)
            {
                if(unit.TeamType != teamType) continue;
                if(unit.Id != unitId) continue;
                return unit;
            }

            return null;
        }
    }
}