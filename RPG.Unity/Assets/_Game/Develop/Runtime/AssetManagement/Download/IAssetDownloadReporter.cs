using System;

namespace PleasantlyGames.RPG.Runtime.AssetManagement.Download
{
    public interface IAssetDownloadReporter : IProgress<float>
    {
        float Progress { get; }
        event Action ProgressUpdate;
        void Reset();
    }
}