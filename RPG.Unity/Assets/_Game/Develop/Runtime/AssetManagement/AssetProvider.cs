#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
#endif

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement.Definition;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using VContainer;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.AssetManagement
{
    public partial class AssetProvider : IAssetProvider
    {
        private readonly Dictionary<string, AsyncOperationHandle> _completedCache = new();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();
        
        [Inject] private AssetProviderDefinition _definition;

        public event Action OnLoadingError;

        [Preserve]
        public AssetProvider()
        {
            
        }

        public void Release(string key)
        {
#if UNITY_WEBGL
            return;
#endif
            if (_handles.TryGetValue(key, out var handleList))
            {
                foreach (var handle in handleList) 
                    Addressables.Release(handle);
                handleList.Clear();
                _handles.Remove(key);   
            }
            _completedCache.Remove(key);
        }
        
        public async UniTask<bool> KeyExistsAsync(string key)
        {
            var handle = Addressables.LoadResourceLocationsAsync(key);
            await handle.Task;
            bool exists = handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0;
            Addressables.Release(handle);
            return exists;
        }

        public async UniTask<IList<IResourceLocation>> GetResourceLocationsAsync(string label, Addressables.MergeMode mode = Addressables.MergeMode.Union, Type type = null)
        {
            IEnumerable<string> keys = new List<string> { label };
            return await Addressables.LoadResourceLocationsAsync(keys, mode, type).ToUniTask();
        }

        public async UniTask<string> GetResourceLocation(AssetReference assetReference)
        {
            var result = await Addressables.LoadResourceLocationsAsync(assetReference.RuntimeKey);
            return result[0].PrimaryKey;
        }

        public UniTask<T[]> WarmUp<T>(IList<IResourceLocation> locations) where T : Object
        {
            List<UniTask<T>> loadTasks = new List<UniTask<T>>(locations.Count);
            foreach (var location in locations) 
                loadTasks.Add(LoadAssetAsync<T>(location, false));

            return UniTask.WhenAll(loadTasks);
        }

        public UniTask<T[]> WarmUp<T>(List<AssetReferenceT<T>> content) where T : Object
        {
            List<UniTask<T>> loadTasks = new List<UniTask<T>>(content.Count);
            foreach (var assetReference in content) 
                loadTasks.Add(LoadAssetAsync<T>(assetReference, false));
            
            return UniTask.WhenAll(loadTasks);
        }

        public async UniTask<T> LoadAssetAsync<T>(AssetReference assetReference, bool showUploading = true) where T : class
        {
            if (_completedCache.TryGetValue(assetReference.AssetGUID, out var completedHandle))
                return completedHandle.Result as T;
            
            return await RunWithCacheOnComplete(Addressables.LoadAssetAsync<T>(assetReference), assetReference.AssetGUID, showUploading);
        }

        public async UniTask<T> LoadAssetAsync<T>(string path, bool showUploading = true) where T : class
        {
            if (_completedCache.TryGetValue(path, out var completedHandle))
                return completedHandle.Result as T;
            
            return await RunWithCacheOnComplete(Addressables.LoadAssetAsync<T>(path), path, showUploading);
        }

        private async UniTask<T> LoadAssetAsync<T>(IResourceLocation resourceLocation, bool showUploading = true) where T : Object
        {
            if (_completedCache.TryGetValue(resourceLocation.PrimaryKey, out var completedHandle))
                return completedHandle.Result as T;
            
            return await RunWithCacheOnComplete(Addressables.LoadAssetAsync<T>(resourceLocation), resourceLocation.PrimaryKey, showUploading);
        }

        private async UniTask<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string key, bool showUploading = true) where T : class
        {
            if (showUploading)
                StartUpload();
            handle.Completed += completeHandle =>
            {
                if(handle.Status == AsyncOperationStatus.Succeeded)
                    _completedCache[key] = completeHandle;
                else
                    OnLoadingError?.Invoke();
            };
            AddHandle(key, handle);
            var result = await handle.ToUniTask();
#if RPG_DEV
            if (showUploading && _definition.UploadingDurationOffset > 0) 
                await UniTask.Delay((int)(_definition.UploadingDurationOffset * 1000), DelayType.UnscaledDeltaTime);
#endif
            
            if (showUploading)
                StopUpload();
            
            return result;
        }

        private void AddHandle<T>(string key, AsyncOperationHandle<T> handle) where T : class
        {
            if (!_handles.TryGetValue(key, out var resourceHandles))
            {
                resourceHandles = new List<AsyncOperationHandle>();
                _handles[key] = resourceHandles;
            }

            resourceHandles.Add(handle);
        }

        public void CleanUp()
        {
            foreach (var resourceHandles in _handles.Values)
            foreach (var resourceHandle in resourceHandles) 
                    Addressables.Release(resourceHandle);
            
            _completedCache.Clear();
            _handles.Clear();
        }

        #region Editor
        public static bool HasAsset(string path)
        {
#if !UNITY_EDITOR
            return false;
#else
            return IsAssetInAddressable(path);
#endif
        }

#if UNITY_EDITOR
        private static bool IsAssetInAddressable(string assetName)
        {
            AddressableAssetSettings settings = GetSettings();
            if (settings == null) return false;
            
            foreach (AddressableAssetGroup group in settings.groups)
            foreach (AddressableAssetEntry entry in group.entries)
                if (entry.address == assetName)
                    return true;

            Debug.LogError($"Asset by key {assetName} does not exists in Addressable");
            return false;
        }

        private static AddressableAssetSettings GetSettings()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Can`t find Addressable settings");
                return null;
            }

            return settings;
        }
#endif
        #endregion
    }
}