using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.AssetManagement
{
    public interface IAssetProvider
    {
        event Action OnLoadingError;
        event Action OnUploadingStop;
        event Action OnUploadingStart;
        UniTask<bool> KeyExistsAsync(string key);
        UniTask<IList<IResourceLocation>> GetResourceLocationsAsync(string label, Addressables.MergeMode mode = Addressables.MergeMode.Union, Type type = null);
        UniTask<T[]> WarmUp<T>(IList<IResourceLocation> locations) where T : Object;
        UniTask<T[]> WarmUp<T>(List<AssetReferenceT<T>> content) where T : Object;
        UniTask<T> LoadAssetAsync<T>(string path, bool showUploading = true) where T : class;
        UniTask<T> LoadAssetAsync<T>(AssetReference assetReference, bool showUploading = true) where T : class;
        void Release(string key);
    }
}