using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PrimeTween;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits
{
    public class BootstrapInitializeLoadUnit : ILoadUnit
    {
        private readonly TimeService _timeService;
        private readonly IInAppProvider _inAppProvider;

        private const int TargetFramerate = 60;
        private const int TargetPhysicsFramerate = 50;
        
        public string DescriptionToken => "Bootstrap";

        [Preserve]
        public BootstrapInitializeLoadUnit(TimeService timeService, IInAppProvider inAppProvider)
        {
            _timeService = timeService;
            _inAppProvider = inAppProvider;
        }

        public UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.75f);
#if !UNITY_EDITOR
            Application.targetFrameRate = TargetFramerate;
#endif
            PrimeTweenConfig.warnTweenOnDisabledTarget = false;
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Time.fixedDeltaTime = (float) 1 / TargetPhysicsFramerate;
            
            _timeService.Initialize();
            _inAppProvider.Initialize();
            
            progress?.Report(1);
            
            return UniTask.CompletedTask;
        }
    }
}