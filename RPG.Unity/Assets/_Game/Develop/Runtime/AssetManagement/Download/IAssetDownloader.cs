using Cysharp.Threading.Tasks;

namespace PleasantlyGames.RPG.Runtime.AssetManagement.Download
{
    public interface IAssetDownloader
    {
        UniTask InitializeAsync();
        float GetDownloadSizeMb();
        UniTask UpdateContentAsync();
        UniTask UpdateDownloadSizeAsync();
        UniTask UpdateCatalogsAsync();
    }
}