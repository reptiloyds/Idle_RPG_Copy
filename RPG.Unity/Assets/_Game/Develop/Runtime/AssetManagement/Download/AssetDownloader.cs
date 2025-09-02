using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.AssetManagement.Download
{
    public class AssetDownloader : IAssetDownloader
    {
        private List<IResourceLocator> _catalogLocators;
        private long _downloadSize;

        private readonly IAssetDownloadReporter _reporter;

        [UnityEngine.Scripting.Preserve]
        [Inject]
        public AssetDownloader(IAssetDownloadReporter reporter) => 
            _reporter = reporter;

        public async UniTask InitializeAsync()
        {
            await Addressables.InitializeAsync().ToUniTask();
        }

        public float GetDownloadSizeMb() => 
            SizeToMb(_downloadSize);

        public async UniTask UpdateContentAsync()
        {
            if (_catalogLocators == null) 
                await UpdateCatalogsAsync();
            
            IList<IResourceLocation> locations = await RefreshResourceLocations(_catalogLocators);

            if (locations == null || locations.Count == 0)
                return;

            await DownloadContentWithPreciseProgress(locations);
        }

        public async UniTask UpdateCatalogsAsync()
        {
            try
            {
                var catalogsToUpdate = await Addressables.CheckForCatalogUpdates().ToUniTask();
                if (catalogsToUpdate == null || catalogsToUpdate.Count == 0)
                {
                    _catalogLocators = Addressables.ResourceLocators.ToList();
                    Logger.Log("Addressables: No catalogs to update");
                    return;
                }

                _catalogLocators = await Addressables.UpdateCatalogs(catalogsToUpdate).ToUniTask();
                Logger.Log("Addressables: Catalogs updated");
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.Message);
            }
        }

        public async UniTask UpdateDownloadSizeAsync()
        {
            IList<IResourceLocation> locations = await RefreshResourceLocations(_catalogLocators);

            if (locations == null || locations.Count == 0)
                return;
            
            _downloadSize = await Addressables.GetDownloadSizeAsync(locations).ToUniTask();
        }

        private async UniTask DownloadContent(IList<IResourceLocation> locations)
        {
            UniTask downloadTask = Addressables
                .DownloadDependenciesAsync(locations, true)
                .ToUniTask(progress: _reporter);

            await downloadTask;

            if (downloadTask.Status.IsFaulted()) 
                Debug.LogError("Error while downloading catalog dependencies");

            _reporter.Reset();
        }

        private async UniTask DownloadContentWithPreciseProgress(IList<IResourceLocation> locations)
        {
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(locations);
            while (!downloadHandle.IsDone && downloadHandle.IsValid())
            {
                await UniTask.Delay(100);
                _reporter.Report(downloadHandle.GetDownloadStatus().Percent);
            }
            
            _reporter.Report(1);
            if (downloadHandle.Status == AsyncOperationStatus.Failed) 
                Debug.LogError("Error while downloading catalog dependencies");

            if (downloadHandle.IsValid()) 
                Addressables.Release(downloadHandle);
            
            _reporter.Reset();
        }

        private static async UniTask<IList<IResourceLocation>> RefreshResourceLocations(IEnumerable<IResourceLocator> locators)
        {
            IEnumerable<object> keysToCheck = locators.SelectMany(x => x.Keys);
            
            return await Addressables
                .LoadResourceLocationsAsync(keysToCheck, Addressables.MergeMode.Union)
                .ToUniTask();
        }

        private static float SizeToMb(long downloadSize) => 
            downloadSize * 1f / 1048576;
    }
}