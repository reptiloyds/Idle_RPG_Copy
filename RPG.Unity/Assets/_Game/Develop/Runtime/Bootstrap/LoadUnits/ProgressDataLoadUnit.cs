using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits
{
    public class ProgressDataLoadUnit : ILoadUnit
    {
        private readonly IDataRepository _dataRepository;
        private readonly ISaveService _saveService;
        private IAnalyticsService _analytics;

        public string DescriptionToken => "ProgressLoading";

        [Preserve]
        public ProgressDataLoadUnit(IDataRepository dataRepository, ISaveService saveService, IAnalyticsService analytics)
        {
            _analytics = analytics;
            _dataRepository = dataRepository;
            _saveService = saveService;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            await _dataRepository.LoadAsync();
            _saveService.Load();
            progress?.Report(1);
            Logger.Log("Progress loaded");
            _analytics.SendProgressLoaded();
        }
    }
}