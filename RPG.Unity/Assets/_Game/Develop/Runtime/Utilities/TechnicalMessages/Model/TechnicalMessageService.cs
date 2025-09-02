using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window.Type;
using PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.Model
{
    public class TechnicalMessageService
    {
        [Inject] private IObjectResolver _resolver;
        [Inject] private UIFactory _uiFactory;
        
        private readonly Dictionary<string, TechnicalMessage> _cached = new();
        private readonly Dictionary<string, TechnicalMessage> _opened = new();
        private readonly Dictionary<string, AsyncLazy<TechnicalMessage>> _inLoading = new();

        [Preserve]
        public TechnicalMessageService()
        {
            
        }
        
        public async UniTask<T> GetAsync<T>() where T : TechnicalMessage
        {
            var windowType = typeof(T).Name;
            if (!_inLoading.TryGetValue(windowType, out var lazyLoading))
            {
                lazyLoading = GetPrefabAsync(windowType).ToAsyncLazy();
                _inLoading.Add(windowType, lazyLoading);
            }
            var window = await lazyLoading;
            _inLoading.Remove(windowType);
            return (T)window;
        }
        
        private async UniTask<TechnicalMessage> GetPrefabAsync(string windowType)
        {
            if (!_cached.TryGetValue(windowType, out var windowPrefab))
            {
                var windowGameObject = await _uiFactory.LoadAsync(windowType, false);
                windowPrefab = windowGameObject.GetComponent<TechnicalMessage>();
                _cached[windowType] = windowPrefab;
            }
            return windowPrefab;
        }
        
        public async UniTask Open<T>() where T : TechnicalMessage
        {
            var key = typeof(T).Name;
            if(_opened.ContainsKey(key)) return;
            if (!_cached.TryGetValue(key, out var messagePrefab))
            {
                var prefabObj = await GetAsync<T>();
                messagePrefab = prefabObj.GetComponent<T>();
                _cached[key] = messagePrefab;
            }

            var message = _resolver.Instantiate(messagePrefab);
            
            _opened[key] = message;
            message.Open();
            message.OnClosed += OnClosed;
        }

        public void Close<T>() where T : TechnicalMessage
        {
            var key = typeof(T).Name;
            if (!_opened.TryGetValue(key, out var message)) return;
            message.Close();
            _opened.Remove(key);
        }

        private void OnClosed(TechnicalMessage technicalMessage) => 
            Object.Destroy(technicalMessage.gameObject);
    }
}