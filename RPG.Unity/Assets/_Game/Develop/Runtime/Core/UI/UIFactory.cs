using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Device;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DeviceType = PleasantlyGames.RPG.Runtime.Device.DeviceType;

namespace PleasantlyGames.RPG.Runtime.Core.UI
{
    public class UIFactory : IInitializable
    {
        [Inject] private IAssetProvider _assetProvider;
        [Inject] private IDeviceService _deviceService;
        
        private string _orientationKey;

        void IInitializable.Initialize()
        {
            var deviceType = _deviceService.GetOrientation();
            switch (deviceType)
            {
                case OrientationType.Vertical:
                    _orientationKey = "Port";
                    break;
                case OrientationType.Horizontal:
                    _orientationKey = "Land";
                    break;
                default:
                    _orientationKey = string.Empty;
                    break;
            }
        }
        
        public async UniTask<Dictionary<string, BaseWindow>> WarmUpWindowsAsync()
        {
            var locations = await _assetProvider.GetResourceLocationsAsync(AssetLabel.WarmUpWindow);
            var results = await _assetProvider.WarmUp<GameObject>(locations);
            var dictionary = new Dictionary<string, BaseWindow>();
            for (var i = 0; i < locations.Count; i++)
            {
                var window = results[i].GetComponent<BaseWindow>();
                dictionary[locations[i].PrimaryKey] = window;
            }
            
            return dictionary;
        }

        public async UniTask<GameObject> LoadAsync(string key, bool isImportant)
        {
            if (!string.IsNullOrEmpty(_orientationKey))
            {
                var orientedKey = $"{key}_{_orientationKey}";
                var orientedKetExists = await _assetProvider.KeyExistsAsync(orientedKey);
                key = orientedKetExists ? orientedKey : key;   
            }
            return await _assetProvider.LoadAssetAsync<GameObject>(key, isImportant);
        }

        public void Release(string key) => 
            _assetProvider.Release(key);
    }
}