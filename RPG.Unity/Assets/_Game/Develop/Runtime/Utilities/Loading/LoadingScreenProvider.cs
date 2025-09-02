using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Utilities.Loading
{
    public class LoadingScreenProvider
    {
        private readonly UIFactory _uiFactory;
        private readonly ITranslator _translator;
        private IObjectResolver _objectResolver;
        
        private LoadingScreen _prefab;
        private LoadingScreen _loadingScreen;
        private CancellationToken _token;

        private readonly List<(Queue<ILoadUnit> queue, UniTaskCompletionSource completionSource)> _pool = new(1);

        [Preserve]
        [Inject]
        public LoadingScreenProvider(UIFactory uiFactory, ITranslator translator, IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
            _uiFactory = uiFactory;
            _translator = translator;
        }

        public async UniTask WarmUp()
        {
            var prefabObj = await _uiFactory.LoadAsync(Asset.UI.LoadingScreen, false);
            _prefab = prefabObj.GetComponent<LoadingScreen>();
        }

        public async UniTask Load(ILoadUnit loadUnit, CancellationToken token)
        {
            var queue = new Queue<ILoadUnit>();
            queue.Enqueue(loadUnit);
            await Load(queue, token);
        }

        public async UniTask Load(Queue<ILoadUnit> loadUnits, CancellationToken token)
        {
            _token = token;
            if (_loadingScreen == null)
            {
                await WarmUp();
                _loadingScreen = _objectResolver.Instantiate(_prefab);
                _loadingScreen.Setup(_translator);
                _loadingScreen.OnComplete += OnLoadingComplete;
                Object.DontDestroyOnLoad(_loadingScreen);   
            }

            if (_loadingScreen.IsLoading)
            {
                var completionSource = new UniTaskCompletionSource();
                PoolUnits(loadUnits, completionSource);
                await completionSource.Task;
            }
            else
                await _loadingScreen.Load(loadUnits, _token);
        }

        public async UniTaskVoid Hide()
        {
            if (_loadingScreen != null)
            {
                _loadingScreen.OnComplete -= OnLoadingComplete;
                await _loadingScreen.Hide();
                Object.Destroy(_loadingScreen.gameObject);
                _loadingScreen = null;
                _prefab = null;
                _uiFactory.Release(Asset.UI.LoadingScreen);
            }
        }

        private async void OnLoadingComplete()
        {
            if(_pool.Count == 0) return;
            if(_token.IsCancellationRequested) return;
            
            var tuple = _pool[0];
            _pool.RemoveAt(0);
            await _loadingScreen.Load(tuple.queue, _token);
            tuple.completionSource.TrySetResult();
        }

        private void PoolUnits(Queue<ILoadUnit> loadUnits, UniTaskCompletionSource completionSource) => 
            _pool.Add((loadUnits, completionSource));
    }
}