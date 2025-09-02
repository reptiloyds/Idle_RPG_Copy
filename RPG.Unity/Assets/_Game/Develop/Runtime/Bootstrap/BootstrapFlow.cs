using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Localization.LoadUnits;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.LoadUnits;
using PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Bootstrap
{
    public class BootstrapFlow : IStartable
    {
        private const string CoreSceneName = "1.Core";
        
        [Inject] private readonly IBootstrapStarter _starter;
        [Inject] private readonly CompositeLoadUnit _platformServicesLoadUnit;
        [Inject] private readonly TranslationLoadUnit _translationLoadUnit;
        
        [Inject] private readonly ITranslator _translator;
        [Inject] private readonly IDataRepository _dataRepository;
        [Inject] private readonly IInAppProvider _inAppProvider;
        [Inject] private readonly LoadingScreenProvider _loadingScreenProvider;
        [Inject] private readonly IAudioService _audio;
        [Inject] private readonly TimeService _timeService;
        [Inject] private readonly InternetConnectionService _connectionService;
        [Inject] private readonly ThirdPartyEvents _thirdPartyEvents;
        [Inject] private readonly IAnalyticsService _analytics;
        
        private AsyncLazy _loadingRequest;
        private CancellationTokenSource _cts;

        [Preserve]
        public BootstrapFlow()
        {
            
        }

        public async void Start()
        {
            _connectionService.OnConnectionLost += OnConnectionLost;
            _thirdPartyEvents.OnInitializationFailed += OnInitializationFailed;
            
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            
            _audio.Initialize();
            _translator.DetectLanguage();
            
            var warmUp = new BootstrapWarmUpLoadUnit(_loadingScreenProvider);
            var internetConnection = new InternetConnectionLoadUnit(_connectionService, _analytics);
            var analyticsInitialization = new AnalyticsLoadUnit(_analytics);
            var composite = new CompositeLoadUnit(new ILoadUnit[]
            {
                new BootstrapInitializeLoadUnit(_timeService, _inAppProvider),
                new AudioLoadUnit(_audio),
            }, "ServiceLoading");
            
            var sceneLoad = new SceneLoadUnit(CoreSceneName);
            
            await _starter.WaitForReady();
            await StartGame();
            return;

            async UniTask StartGame()
            {
                await warmUp.LoadAsync(token);
                var loadUnits = new Queue<ILoadUnit>();
                loadUnits.Enqueue(analyticsInitialization);
                loadUnits.Enqueue(_translationLoadUnit);
                loadUnits.Enqueue(internetConnection);
                loadUnits.Enqueue(_platformServicesLoadUnit);
                loadUnits.Enqueue(composite);
                loadUnits.Enqueue(sceneLoad);
                await _loadingScreenProvider.Load(loadUnits, token)
                    .SuppressCancellationThrow();
            }
        }

        private void OnInitializationFailed(bool handle)
        {
            _cts.Cancel();
            Complete();
        }

        private void OnConnectionLost()
        {
            _cts.Cancel();
            Complete();
        }

        private void Complete()
        {
            _connectionService.OnConnectionLost -= OnConnectionLost;
            _thirdPartyEvents.OnInitializationFailed -= OnInitializationFailed;
            
            _connectionService.EnableRarePing();
        }
    }
}