using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GamePush;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.TimeUtility.LoadUnit
{
    public class GamePushServerTimeLoadUnit : ILoadUnit
    {
        [Inject] private TimeService _timeService;
        public string DescriptionToken => "TimeLoading";
        
        public UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            _timeService.SetupServerStartTime(GP_Server.Time());
            progress?.Report(1f);
            return UniTask.CompletedTask;
        }
    }
}