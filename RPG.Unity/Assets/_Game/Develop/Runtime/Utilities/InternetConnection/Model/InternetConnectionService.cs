using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Utilities.ApplicationCloser.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.Definition;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.Model
{
    public class InternetConnectionService : ITickable
    {
        [Inject] private InternetConnectionConfiguration _configuration;
        [Inject] private IApplicationCloser _applicationCloser;
        
        private readonly List<WebRequestPing> _webRequests = new();
        private WebRequestPing _primaryRequest;
        
        private float _pingDelay;
        private float _pingTimer;
        private bool _timerIsActive;
        
        public bool CanRestore { get; private set; }
        public bool HasConnection { get; private set; }
        public event Action OnConnectionLost;
        public event Action OnConnectionRestored;

        [Preserve]
        public InternetConnectionService()
        {
            
        }

        public async UniTask Initialize()
        {
            foreach (var uri in _configuration.Uris) 
                _webRequests.Add(new WebRequestPing(uri));
            await SelectRequest();

            CanRestore = HasConnection;
            if (HasConnection)
            {
                EnableFrequentPing();
                _timerIsActive = true;
            }
            else
                ConnectionLost();
        }
        
        private async UniTask SelectRequest()
        {
            _primaryRequest = null;
            foreach (var request in _webRequests)
            {
                var result = await request.Ping();
                if(!result) continue;
                _primaryRequest = request;
                break;
            }
            
            HasConnection = _primaryRequest != null;
        }

        public void EnableFrequentPing()
        {
#if UNITY_EDITOR
            _pingDelay = 9999999;
#else
            _pingDelay = _configuration.FrequentPingDelay;
#endif
            ResetTimer();
        }

        public void EnableRarePing()
        {
#if UNITY_EDITOR
            _pingDelay = 9999999;
#else
            _pingDelay = _configuration.RarePingDelay;
#endif
            ResetTimer();
        }

        public void RestoreConnection()
        {
            if (CanRestore)
                Ping().Forget();
            else
                _applicationCloser.Close();
        }

        private void ResetTimer() => 
            _pingTimer = _pingDelay;

        void ITickable.Tick()
        {
            if(!_timerIsActive) return;

            _pingTimer -= Time.unscaledDeltaTime;
            if(_pingTimer <= 0) 
                Ping().Forget();
        }

        public async UniTaskVoid Ping()
        {
            if(!_timerIsActive) return;
            _timerIsActive = false;

            var wasConnected = HasConnection;
            HasConnection = await _primaryRequest.Ping();

            if (wasConnected && !HasConnection) ConnectionLost();
            if (!wasConnected && HasConnection) ConnectionRestored();

            ResetTimer();
            _timerIsActive = true;
        }

        private void ConnectionLost()
        {
            Logger.LogError("Lost internet connection");
            OnConnectionLost?.Invoke();
            EnableFrequentPing();
        }

        private void ConnectionRestored()
        {
            Logger.LogError("Restore internet connection");
            OnConnectionRestored?.Invoke();
            EnableRarePing();
        }
    }
}