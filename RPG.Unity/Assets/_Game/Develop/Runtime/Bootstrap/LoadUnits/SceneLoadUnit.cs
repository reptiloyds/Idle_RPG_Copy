using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits
{
    public class SceneLoadUnit : ILoadUnit
    {
        private readonly string _sceneName;

        public string DescriptionToken => "SceneLoading";

        [Preserve]
        public SceneLoadUnit(string sceneName) => 
            _sceneName = sceneName;

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.85f);
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(_sceneName);
            await handle;
            progress?.Report(1f);
        }
    }
}