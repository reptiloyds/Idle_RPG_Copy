using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamePush;
using GamePush.Data;
using PleasantlyGames.RPG.Runtime.Balance.Const;
using PleasantlyGames.RPG.Runtime.Balance.Contract;
using PleasantlyGames.RPG.Runtime.Balance.Save;
using PleasantlyGames.RPG.YGRuntime.Segments.Model;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.YGRuntime.Balance.Model
{
    public class GamePushBalanceProvider : IInitializable, IDisposable, IBalanceProvider
    {
        private BalanceData _data;
        private UniTaskCompletionSource _tcs;
        
        [Inject] private IBalanceLoader _balanceLoader;
        [Inject] private GamePushSegmentService _segmentService;
        [Inject] private BalanceDataProvider _balanceDataProvider;

        private readonly List<(string segmentName, int Weight)> _weightedSegments = new()
        {
            ("activePlayer", 1),
            ("adWatcher", 2),
            ("donater", 3)
        };

        public string FileName => _data.FileName;
        public event Action OnFileNameChanged;

        [Preserve]
        public GamePushBalanceProvider()
        {
        }

        public void Initialize()
        {
            _segmentService.OnSegmentEntered += OnSegmentEntered;
            _segmentService.OnSegmentLeaved += OnSegmentLeaved;
        }

        public void Dispose()
        {
            _segmentService.OnSegmentEntered -= OnSegmentEntered;
            _segmentService.OnSegmentLeaved -= OnSegmentLeaved;
        }

        private void OnSegmentEntered(string segment) => 
            UpdateBalanceFile();

        private void OnSegmentLeaved(string segment) => 
            UpdateBalanceFile();

        private void UpdateBalanceFile()
        {
            int maxWeight = 0;
            string preferredSegment = null;
            foreach (var tuple in _weightedSegments)
            {
                if (!_segmentService.HasSegment(tuple.segmentName) || tuple.Weight <= maxWeight) continue;
                preferredSegment = tuple.segmentName;
                maxWeight = tuple.Weight;
            }
            
            if (!string.IsNullOrEmpty(preferredSegment))
            {
                _data.FileName = BalanceConst.BalanceNamePrefix + preferredSegment;
                OnFileNameChanged?.Invoke();
            } 
        }

        public async UniTask Load()
        {
            _balanceDataProvider.LoadData();
            _data = _balanceDataProvider.GetData();
#if UNITY_EDITOR
            await _balanceLoader.Load();
            return;
#endif

            UpdateBalanceFile();
            _tcs = new UniTaskCompletionSource();
            GP_Files.OnLoadContent += OnContentLoaded;
            GP_Files.OnLoadContentError += OnLoadContentError;
            GP_Files.LoadContent($"https://s3.eponesh.com/games/files/{ProjectData.ID}/{_data.FileName}.json");
            await _tcs.Task;
        }

        private async void OnContentLoaded(string result)
        {
            GP_Files.OnLoadContent -= OnContentLoaded;
            GP_Files.OnLoadContentError -= OnLoadContentError;
            Debug.LogWarning(result);
            await _balanceLoader.Load(result);
            _tcs.TrySetResult();
        }

        private void OnLoadContentError()
        {
            GP_Files.OnLoadContent -= OnContentLoaded;
            GP_Files.OnLoadContentError -= OnLoadContentError;
            Debug.LogWarning("Fail load balance");
            //TODO HANDLE THIS CASE AND POPUP INFO TO USER
        }
    }
}