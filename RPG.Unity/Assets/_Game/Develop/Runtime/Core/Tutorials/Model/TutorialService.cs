using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Save;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model
{
    public class TutorialService
    {
        [Inject] private BalanceContainer _balanceContainer;
        [Inject] private TutorialDataProvider _dataProvider;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IWindowService _windowService;
        [Inject] private IAnalyticsService _analytics;
        
        private TutorialDataContainer _data;
        private readonly HashSet<Tutorial> _tutorials = new();
        private readonly Queue<Tutorial> _readyTutorials = new();

        private Tutorial _pausedTutorial;
        private Tutorial _activeTutorial;

        public bool HasActiveTutorial => _activeTutorial != null;
        public event Action OnTutorialStarted;
        public event Action OnTutorialCompleted;

        [Preserve]
        public TutorialService() { }

        public async UniTask InitializeAsync()
        {
            _data = _dataProvider.GetData();
            await CreateTutorialsAsync();
        }

        public async UniTaskVoid TestTutorialAsync(string id)
        {
            _activeTutorial?.Pause();
            _activeTutorial = null;
            foreach (var tutorial in _tutorials)
            {
                tutorial.DestroyTrigger();
                if (string.Equals(tutorial.Id, id)) 
                    await LaunchTutorialAsync(tutorial);
            }
        }

        private async UniTask CreateTutorialsAsync()
        {
            var sheet = _balanceContainer.Get<TutorialSheet>();

            foreach (var row in sheet)
            {
                if(_data.CompletedTutorials.Contains(row.Id)) continue;
                var tutorial = new Tutorial(row);
                _objectResolver.Inject(tutorial);

                _tutorials.Add(tutorial);
                if (string.Equals(_data.CurrentTutorialId, row.Id))
                    await StartTutorial(tutorial, _data.CurrentStepOrder);
                else
                {
                    tutorial.OnReadyToStart += OnTutorialReadyToStart;
                    tutorial.Initialize();   
                }
            }
        }

        private async void OnTutorialReadyToStart(Tutorial tutorial)
        {
            tutorial.OnReadyToStart -= OnTutorialReadyToStart;
            
#if RPG_DEV && TUTORIAL_DISABLED
            return;    
#endif

            await LaunchTutorialAsync(tutorial);
        }

        private async UniTask LaunchTutorialAsync(Tutorial tutorial)
        {
            if (tutorial.IsPriority)
            {
                if (_activeTutorial != null)
                {
                    _activeTutorial.Pause();
                    _pausedTutorial = _activeTutorial;
                }
                await StartTutorial(tutorial, 1);
            }
            else
            {
                if(_activeTutorial == null)
                    await StartTutorial(tutorial, 1);
                else
                    _readyTutorials.Enqueue(tutorial);   
            }
        }

        private async UniTask StartTutorial(Tutorial tutorial, int stepOrder)
        {
            _activeTutorial = tutorial;
            _data.CurrentTutorialId = _activeTutorial.Id;
            _analytics.SendTutorialStarted(_data.CurrentTutorialId);
            _activeTutorial.OnComplete += OnTutorialComplete;
            _activeTutorial.OnStartSteps += OnTutorialStartSteps;
            if (_activeTutorial.WarmUpWindows.Count > 0) 
                await _windowService.WarmUpAsync(_activeTutorial.WarmUpWindows, true);
            _activeTutorial.Start(stepOrder);
            
            OnTutorialStarted?.Invoke();
        }

        private void OnTutorialStartSteps(int order) => 
            _data.CurrentStepOrder = order;

        private void OnTutorialComplete(Tutorial tutorial)
        {
            
            _analytics.SendTutorialCompleted(tutorial.Id);
            _data.CompletedTutorials.Add(tutorial.Id);
            _activeTutorial.OnStartSteps -= OnTutorialStartSteps;
            _activeTutorial = null;
            _data.CurrentTutorialId = null;
            _data.CurrentStepOrder = 1;
            
            tutorial.OnComplete -= OnTutorialComplete;
            OnTutorialCompleted?.Invoke();
            if (_pausedTutorial != null)
            {
                _activeTutorial = _pausedTutorial;
                _pausedTutorial = null;
                _activeTutorial.Continue();
                return;
            }
            if(_readyTutorials.Count == 0) return;

            var nextTutorial = _readyTutorials.Dequeue();
            StartTutorial(nextTutorial, 1);
        }
    }
}