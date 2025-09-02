using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Skill.View;
using PleasantlyGames.RPG.Runtime.Pool;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model
{
    public class SkillViewFactory
    {
        private readonly Dictionary<string, SkillView> _viewsCache = new ();
        private readonly Dictionary<string, ObjectPoolWithParent<SkillView>> _poolDictionary = new();
        
        private readonly HashSet<SkillView> _activeViews = new();
        private const int DefaultCapacity = 3;
        private const int PreloadCount = 1;

        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IAssetProvider _assetProvider;

        [Preserve]
        public SkillViewFactory() { }
        
        public async UniTask WarmUpAsync()
        {
            var locations = await _assetProvider.GetResourceLocationsAsync(AssetLabel.SkillView);
            var results = await _assetProvider.WarmUp<GameObject>(locations);
            for (int i = 0; i < locations.Count; i++) 
                _viewsCache.Add(locations[i].PrimaryKey, results[i].GetComponent<SkillView>());
        }

        public SkillView GetView(string key, Vector3 position, Vector3 size = default)
        {
            SkillView result;
            if (_poolDictionary.TryGetValue(key, out var pool))
                result = pool.Get();
            else
            {
                var prefab = _viewsCache[key];
                var newPool = CreatePool(prefab);
                _poolDictionary[key] = newPool;
                result = newPool.Get();
            }
            
            result.Spawn(position, size != default ? size : Vector3.one, key);

            return result;
        }

        private void OnDespawnComplete(SkillView view)
        {
            if (_poolDictionary.TryGetValue(view.Id, out var pool))
                pool.Release(view);
            else
                Debug.LogError("Skill pool not found");   
        }

        private ObjectPoolWithParent<SkillView> CreatePool(SkillView prefab)
        {
            var pool = new ObjectPoolWithParent<SkillView>($"{prefab.name}_Pool", () => CreateView(prefab), ActionOnGet, ActionOnRelease, ActionOnDestroy, DefaultCapacity);
            pool.PreloadObjects(PreloadCount);
            return pool;
        }
        
        private SkillView CreateView(SkillView template)
        {
            var instance = _objectResolver.Instantiate(template);
            return instance;
        }
        
        private void ActionOnRelease(SkillView view)
        {
            view.OnDespawn -= OnDespawnComplete;
            _activeViews.Add(view);
            view.gameObject.SetActive(false);
        }

        private void ActionOnGet(SkillView view)
        {
            view.gameObject.SetActive(true);
            view.OnDespawn += OnDespawnComplete;
            _activeViews.Add(view);
        }

        private void ActionOnDestroy(SkillView view) => 
            Object.Destroy(view);
    }
}