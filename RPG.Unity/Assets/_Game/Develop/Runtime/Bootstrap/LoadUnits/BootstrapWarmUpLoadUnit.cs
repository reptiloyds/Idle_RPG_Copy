using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits
{
    public class BootstrapWarmUpLoadUnit : ILoadUnit
    {
        private readonly LoadingScreenProvider _loadingScreenProvider;
        public string DescriptionToken => "WarmUp";

        [Preserve]
        public BootstrapWarmUpLoadUnit(LoadingScreenProvider loadingScreenProvider)
        {
            _loadingScreenProvider = loadingScreenProvider;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.85f); 
            await _loadingScreenProvider.WarmUp();
            progress?.Report(1);
        }
    }
}