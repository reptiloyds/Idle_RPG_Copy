using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Effect;
using PleasantlyGames.RPG.Runtime.Pool;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Effects.Model
{
    public class EffectFactory
    {
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IAssetProvider _assetProvider;

        private readonly Dictionary<string, BaseEffect> _effectCache = new();
        private readonly Dictionary<string, ObjectPoolWithParent<BaseEffect>> _poolDictionary = new();
        
        private const int DefaultCapacity = 3;
        private const int PreloadCount = 1;

        [Preserve]
        public EffectFactory() { }
        
        public async UniTask WarmUpAsync()
        {
            var locations = await _assetProvider.GetResourceLocationsAsync(AssetLabel.EffectView);
            var warmUpResults = await _assetProvider.WarmUp<GameObject>(locations);
            for (var i = 0; i < locations.Count; i++) 
                _effectCache[locations[i].PrimaryKey] = warmUpResults[i].GetComponent<BaseEffect>();
        }

        public BaseEffect Create(string key, float lifetime = -1)
        {
            BaseEffect result;
            if (_poolDictionary.TryGetValue(key, out var pool))
                result = pool.Get();
            else
            {
                var prefab = _effectCache[key];
                var newPool = CreatePool(prefab);
                _poolDictionary[key] = newPool;
                result = newPool.Get();
            }
            
            result.SetId(key);
            result.OnRelease += Release;
            if (lifetime > 0) result.SetLifetime(lifetime);
            
            return result;
        }

        public void Release(BaseEffect effect)
        {
            effect.OnRelease -= Release;
            
            if (_poolDictionary.TryGetValue(effect.Id, out var pool)) 
                pool.Release(effect);
        }

        private ObjectPoolWithParent<BaseEffect> CreatePool(BaseEffect prefab)
        {
            var pool = new ObjectPoolWithParent<BaseEffect>($"{prefab.name}_Pool", () => CreateView(prefab), ActionOnGet, ActionOnRelease, ActionOnDestroy, DefaultCapacity);
            pool.PreloadObjects(PreloadCount);
            return pool;
        }
        
        private BaseEffect CreateView(BaseEffect template)
        {
            var instance = _objectResolver.Instantiate(template);
            return instance;
        }

        private void ActionOnGet(BaseEffect view) => 
            view.gameObject.SetActive(true);

        private void ActionOnRelease(BaseEffect view) => 
            view.gameObject.SetActive(false);

        private void ActionOnDestroy(BaseEffect view) => 
            Object.Destroy(view);
    }
}