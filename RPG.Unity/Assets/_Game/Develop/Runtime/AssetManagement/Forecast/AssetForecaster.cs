using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Factory;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.AssetManagement.Forecast
{
    public class AssetForecaster : IDisposable
    {
        [Inject] private UnitFactory _unitFactory;
        [Inject] private LocationFactory _locationFactory;
        [Inject] private IEnumerable<IAssetUser> _assetUsers;

        private readonly Dictionary<AssetType, HashSet<string>> _neededAssets;
        
        private readonly Queue<Func<UniTask>> _updateQueue = new();
        private bool _queueProcessing;
        
        [Preserve]
        public AssetForecaster()
        {
            _neededAssets = new Dictionary<AssetType, HashSet<string>>()
            {
                {AssetType.Unit, new HashSet<string>()},
                {AssetType.Location, new HashSet<string>()}
            };
        }

        public void Initialize()
        {
            foreach (var assetUser in _assetUsers) 
                assetUser.OnNeedsChanged += EnqueueUpdate;
        }

        void IDisposable.Dispose()
        {
            foreach (var assetUser in _assetUsers) 
                assetUser.OnNeedsChanged -= EnqueueUpdate;
        }

        public async UniTask Forecast()
        {
            foreach (var kvp in _neededAssets) 
                kvp.Value.Clear();

            foreach (var assetUser in _assetUsers) 
                assetUser.FillNeeds(_neededAssets);
            
            foreach (var kvp in _neededAssets)
            {
                switch (kvp.Key)
                {
                    case AssetType.Unit:
                        await _unitFactory.WarmUpAsync(kvp.Value);
                        break;
                    case AssetType.Location:
                        await _locationFactory.WarmUpAsync(kvp.Value);
                        break;
                }
            }
        }

        private void EnqueueUpdate()
        {
            _updateQueue.Enqueue(Forecast);
            if (!_queueProcessing) 
                ProcessQueue().Forget();
        }

        private async UniTaskVoid ProcessQueue()
        {
            _queueProcessing = true;

            while (_updateQueue.Count > 0)
            {
                var task = _updateQueue.Dequeue();
                await task();
            }

            _queueProcessing = false;
        }
    }
}