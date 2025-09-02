using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement.Download;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.AssetManagement.LoadUnits
{
    public class DeprecatedAssetDownloadUnit : ILoadUnit
    {
        private readonly IAssetDownloader _assetDownloader;

        public string DescriptionToken => "AssetDownloads";

        [Preserve]
        public DeprecatedAssetDownloadUnit(IAssetDownloader assetDownloader) => 
            _assetDownloader = assetDownloader;

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            await _assetDownloader.InitializeAsync();
            await _assetDownloader.UpdateCatalogsAsync();
            await _assetDownloader.UpdateDownloadSizeAsync();
            float downloadSize = _assetDownloader.GetDownloadSizeMb();
            if (downloadSize > 0) 
                await _assetDownloader.UpdateContentAsync();
            progress?.Report(1);
        }
    }
}