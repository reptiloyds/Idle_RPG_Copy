using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Location.View;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Core.Location.Model
{
    public class LocationFactory
    {
        private readonly Dictionary<string, LocationView> _prefabs = new();
        private LocationView _location;
        private string _locationKey;
        
        private readonly List<string> _keysToRemove = new(5);
        
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IAssetProvider _assetProvider;

        public LocationView Location => _location;
        public event Action OnLocationLoaded;
        
        [Preserve]
        public LocationFactory() { }

        public async UniTask WarmUpAsync(HashSet<string> locations)
        {
            foreach (var key in _prefabs.Keys)
                if (!locations.Contains(key)) 
                    _keysToRemove.Add(key);

            foreach (var keyToRemove in _keysToRemove)
            {
                if (_locationKey == keyToRemove) 
                    ClearCurrentLocation();
                _prefabs.Remove(keyToRemove);
                _assetProvider.Release(keyToRemove);
            }
            
            await UniTask.WhenAll(locations.Select(WarmUpLocationAsync));
        }
        
        private async UniTask WarmUpLocationAsync(string locationKey) => 
            await LoadLocationAsync(locationKey, true);

        private async UniTask LoadLocationAsync(string locationKey, bool isWarmUp)
        {
            if (_prefabs.ContainsKey(locationKey)) return;
            var unitObject = await _assetProvider.LoadAssetAsync<GameObject>(locationKey, !isWarmUp);
            if (unitObject.TryGetComponent(out LocationView location)) 
                _prefabs[locationKey] = location;
        }
        
        public async UniTask CreateAsync(string locationKey)
        {
            if (string.Equals(_locationKey, locationKey))
            {
                ClearCurrentLocation();
                OnLocationLoaded?.Invoke();
                return;
            }
            if(_location != null)
                Release();
            
            if (!_prefabs.ContainsKey(locationKey)) 
                await LoadLocationAsync(locationKey, false); 
            
            _location = _objectResolver.Instantiate(_prefabs[locationKey]);
            _locationKey = locationKey;
            
            OnLocationLoaded?.Invoke();
        }

        public void ClearCurrentLocation() => 
            _location?.Clear();

        private void Release()
        {
            _location.Clear();
            Object.Destroy(_location.gameObject);
            _location = null;
            _locationKey = null;
        }
    }
}