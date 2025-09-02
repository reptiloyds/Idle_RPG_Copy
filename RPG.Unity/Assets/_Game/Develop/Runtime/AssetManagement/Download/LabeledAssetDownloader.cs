using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.AssetManagement.Download
{
    public class LabeledAssetDownloader : IAssetDownloader
    {
        public const string RemoteLabel = "remote";
        
        private long _downloadSize;

        [Inject] private IAssetDownloadReporter _reporter;

        public async UniTask InitializeAsync()
        {
            //AssetBundle.memoryBudgetKB = 256 * 1024;
            await Addressables.InitializeAsync().ToUniTask();
            await UpdateCatalogsAsync();
            await UpdateDownloadSizeAsync();
        }

        public float GetDownloadSizeMb() => 
            SizeToMb(_downloadSize);

        public async UniTask UpdateContentAsync()
        {
            try
            {
                AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(RemoteLabel);
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
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public async UniTask UpdateCatalogsAsync()
        {
            List<string> catalogsToUpdate = await Addressables.CheckForCatalogUpdates().ToUniTask();
            if (catalogsToUpdate == null || catalogsToUpdate.Count == 0)
                return;

            await Addressables.UpdateCatalogs(catalogsToUpdate).ToUniTask();
        }

        public async UniTask UpdateDownloadSizeAsync()
        {
            _downloadSize = await Addressables
                .GetDownloadSizeAsync(RemoteLabel)
                .ToUniTask();
        }
        
        private static float SizeToMb(long downloadSize) => 
            downloadSize * 1f / 1048576;
    }
}