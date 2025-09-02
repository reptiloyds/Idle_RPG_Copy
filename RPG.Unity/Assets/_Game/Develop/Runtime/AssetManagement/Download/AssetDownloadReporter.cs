using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.AssetManagement.Download
{
    public class AssetDownloadReporter : IAssetDownloadReporter
    {
        public float Progress { get; private set; }
        public event Action ProgressUpdate;

        [Preserve]
        public AssetDownloadReporter()
        {
            
        }
        
        public void Report(float value)
        {
            if(Mathf.Approximately(Progress, value))
                return;
            
            Progress = value;
            ProgressUpdate?.Invoke();
        }

        public void Reset()
        {
            Progress = 0;
        }
    }
}