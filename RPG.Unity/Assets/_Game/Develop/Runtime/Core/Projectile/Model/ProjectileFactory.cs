using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Projectile.View;
using PleasantlyGames.RPG.Runtime.Pool;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Core.Projectile.Model
{
    public class ProjectileFactory : IDisposable
    {
        private readonly Dictionary<string, GameObject> _viewCacheDictionary = new();
        private BaseProjectile _projectilePrefab;
        private readonly Dictionary<string, ObjectPoolWithParent<Transform>> _viewPoolDictionary = new();
        private readonly List<BaseProjectile> _projectiles = new();
        private readonly HashSet<GameObject> _activeView = new();
        
        private ObjectPoolWithParent<BaseProjectile> _projectilePool;

        [Inject] private IAssetProvider _assetProvider;
        
        private const int DefaultCapacity = 3;
        private const int PreloadCount = 1;

        [Preserve]
        public ProjectileFactory() { }

        public async UniTask WarmUpAsync()
        {
            UniTask<GameObject> projectileLoadTask = _assetProvider.LoadAssetAsync<GameObject>(Asset.BaseProjectile, false);

            var locations = await _assetProvider.GetResourceLocationsAsync(AssetLabel.ProjectileView);
            UniTask<GameObject[]> warmUpTask = _assetProvider.WarmUp<GameObject>(locations);
            (GameObject baseProjectile, GameObject[] projectileViews) loadResult = await (projectileLoadTask, warmUpTask);
            
            _projectilePrefab = loadResult.baseProjectile.GetComponent<BaseProjectile>();
            for (var i = 0; i < locations.Count; i++) 
                _viewCacheDictionary[locations[i].PrimaryKey] = loadResult.projectileViews[i];
        }

        public void Initialize() => 
            CreateProjectilePool(_projectilePrefab);

        void IDisposable.Dispose()
        {
            // for (var i = _projectiles.Count - 1; i >= 0; i--) 
            //     ReleaseProjectile(_projectiles[i]);
        }

        public GameObject CreateParticle(string key) => 
            CreateView(key);

        public void ReleaseParticle(string key, GameObject view) => 
            ReleaseView(key, view);

        public BaseProjectile CreateProjectile(string key = null)
        {
            BaseProjectile baseProjectile = _projectilePool.Get();
            
            if (!string.IsNullOrEmpty(key))
            {
                var view = CreateView(key);
                baseProjectile.SetVisual(view.gameObject, key);
            }
            
            baseProjectile.Initialize();
            return baseProjectile;
        }

        public void ReleaseProjectile(BaseProjectile projectile)
        {
            if (!string.IsNullOrEmpty(projectile.VisualId)) 
                ReleaseView(projectile.VisualId, projectile.Visual);
            
            projectile.ClearVisual();
            _projectilePool.Release(projectile);
        }

        private GameObject CreateView(string key)
        {
            Transform result;
            if (_viewPoolDictionary.TryGetValue(key, out var pool))
                result = pool.Get();
            else
            {
                var prefab = _viewCacheDictionary[key];
                var newPool = CreateViewPool(prefab);
                _viewPoolDictionary[key] = newPool;
                result = newPool.Get();
            }
            
            return result.gameObject;
        }

        private void ReleaseView(string key, GameObject view)
        {
            if (_viewPoolDictionary.TryGetValue(key, out var pool) && _activeView.Contains(view)) 
                pool.Release(view.transform);
        }

        private void CreateProjectilePool(BaseProjectile prefab)
        {
            _projectilePool = new ObjectPoolWithParent<BaseProjectile>("Projectile_Pool", () => Object.Instantiate(prefab), ActionOnGet, ActionOnRelease, ActionOnDestroy);
            
            void ActionOnGet(BaseProjectile projectile)
            {
                projectile.gameObject.SetActive(true);
                _projectiles.Add(projectile);
            }

            void ActionOnRelease(BaseProjectile projectile)
            {
                projectile.gameObject.SetActive(false);
                _projectiles.Remove(projectile);
            }

            void ActionOnDestroy(BaseProjectile projectile) => 
                Object.Destroy(projectile);
        }

        private ObjectPoolWithParent<Transform> CreateViewPool(GameObject prefab)
        {
            var pool = new ObjectPoolWithParent<Transform>($"{prefab.name}_Pool", () => Object.Instantiate(prefab).transform, ActionOnGet, ActionOnRelease, ActionOnDestroy, DefaultCapacity);
            pool.PreloadObjects(PreloadCount);
            return pool;
            
            void ActionOnGet(Transform view)
            {
                view.gameObject.SetActive(true);
                _activeView.Add(view.gameObject);
            }

            void ActionOnRelease(Transform view)
            {
                view.gameObject.SetActive(false);
                _activeView.Remove(view.gameObject);
            }

            void ActionOnDestroy(Transform view) => 
                Object.Destroy(view);
        }
    }
}