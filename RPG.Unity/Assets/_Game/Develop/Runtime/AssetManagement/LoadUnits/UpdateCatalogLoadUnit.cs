using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.AssetManagement.Download;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.AssetManagement.LoadUnits
{
    public class UpdateCatalogLoadUnit : ILoadUnit
    {
        private readonly IAssetDownloader _assetDownloader;
        private IAnalyticsService _analytics;

        public string DescriptionToken => "UpdateCatalogLoadUnit";

        [Preserve]
        public UpdateCatalogLoadUnit(IAssetDownloader assetDownloader, IAnalyticsService analytics)
        {
            _analytics = analytics;
            _assetDownloader = assetDownloader;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            await _assetDownloader.InitializeAsync();
            await _assetDownloader.UpdateCatalogsAsync();
            progress?.Report(1f);
            _analytics.SendCatalogsUpdated();
        }
    }
}