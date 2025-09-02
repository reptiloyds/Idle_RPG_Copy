using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Accent;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.AppendResource;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.CloseWindow;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Empty;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Monologue;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.OpenWindow;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.PlayAnimation;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Pointer;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.SwitchBranch;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Button;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Content;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.MainMode;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Quest;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract;
using PrimeTween;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model
{
    public class Tutorial
    {
        [Inject] private QuestService _questService;
        [Inject] private MainMode _mainMode;
        [Inject] private IButtonService _buttonService;
        [Inject] private IObjectResolver _resolver;
        [Inject] private ContentService _contentService;
        [Inject] private IAnalyticsService _analytics;
        
        private readonly TutorialRow _config;
        private TutorialTrigger _trigger;
        private readonly List<TutorialStep> _steps = new(10);
        private readonly HashSet<TutorialStep> _activeSteps = new(3);
        private bool _isSavePaused;
        
        private bool _isRunning;
        
        private readonly Dictionary<TutorialStep, Tween> _delayedSteps = new();
        private const int DEFAULT_START_ORDER = 1;
        private int _launchStepOrder;

        public IReadOnlyList<string> WarmUpWindows => _config.WarmUpWindowsList;
        public bool IsPriority => _config.Priority;
        public string Id => _config.Id;

        public event Action<int> OnStartSteps;
        public event Action<Tutorial> OnReadyToStart;
        public event Action<Tutorial> OnComplete;
        
        public Tutorial(TutorialRow config) => 
            _config = config;

        public void Initialize() => 
            CreateTrigger();

        public void Start(int order)
        {
            if (order < DEFAULT_START_ORDER)
                order = DEFAULT_START_ORDER;
            _launchStepOrder = order;
            _isRunning = true;
            CreateSteps();
            NextStepCluster();
        }

        public void Pause()
        {
            if(!_isRunning) return;
            _isRunning = false;
            
            foreach (var step in _activeSteps)
            {
                //_buttonService.RemoveAllowedButtonIds(step.AllowedButtons);
                step.Pause();
            }

            foreach (var kvp in _delayedSteps)
            {
                var tween = kvp.Value;
                tween.isPaused = true;
            } 
        }

        public void Continue()
        {
            if(_isRunning) return;
            _isRunning = true;
            
            foreach (var step in _activeSteps) 
                step.Continue();

            foreach (var kvp in _delayedSteps)
            {
                var tween = kvp.Value;
                tween.isPaused = false;
            }
        }

        private void CreateSteps()
        {
            foreach (var tutorialElem in _config)
            {
                if(tutorialElem.Order < _launchStepOrder) continue;
                TutorialStep step = null;
                switch (tutorialElem.StepType)
                {
                    case TutorialStepType.Empty:
                        step = new EmptyTutorialStep(tutorialElem);
                        break;
                    case TutorialStepType.Monologue:
                        step = new MonologueTutorialStep(tutorialElem);
                        break;
                    case TutorialStepType.Pointer:
                        step = new PointerTutorialStep(tutorialElem);
                        break;
                    case TutorialStepType.Accent:
                        step = new AccentTutorialStep(tutorialElem);
                        break;
                    case TutorialStepType.AppendResource:
                        step = new AppendResourceStep(tutorialElem);
                        break;
                    case TutorialStepType.PlayAnimation:
                        step = new PlayAnimationStep(tutorialElem);
                        break;
                    case TutorialStepType.OpenWindow:
                        step = new OpenWindowStep(tutorialElem);
                        break;
                    case TutorialStepType.CloseWindow:
                        step = new CloseWindowStep(tutorialElem);
                        break;
                    case TutorialStepType.SwitchBranch:
                        step = new SwitchBranchTutorialStep(tutorialElem);
                        break;
                }
                _steps.Add(step);
                
                _resolver.Inject(step);
            }
        }

        private void NextStepCluster()
        {
            var stepId = 0;
            var step = _steps[stepId];
            var order = step.Order;
            do
            {
                var lateActivationStep = step;
                step.PreStart(DEFAULT_START_ORDER == step.Order, _launchStepOrder == step.Order);
                step.Completed += OnStepCompleted;
                if (step.StartDelay > 0)
                {
                    var tween = Tween.Delay(step.StartDelay, () => StartDelayedStep(lateActivationStep), useUnscaledTime: true);
                    _delayedSteps.Add(step, tween);   
                }
                else
                {
                    step.Start();
                    _analytics.SendTutorialStepStarted(Id, step.Order);
                    _activeSteps.Add(step);
                }
                
                stepId++;
                if (stepId >= _steps.Count) break;
                step = _steps[stepId];
            }
            while (step.Order == order);
            OnStartSteps?.Invoke(order);
        }

        private void StartDelayedStep(TutorialStep step)
        {
            if(_delayedSteps.ContainsKey(step))
                _delayedSteps.Remove(step);
            // _buttonService.AppendAllowedButtonIds(step.AllowedButtons);

            _analytics.SendTutorialStepStarted(Id, step.Order);
            _activeSteps.Add(step);
            step.Start();
        }

        private void OnStepCompleted(TutorialStep step)
        {
            step.Completed -= OnStepCompleted;
            // _buttonService.RemoveAllowedButtonIds(step.AllowedButtons);
            step.Dispose();
            
            _steps.RemoveAt(0);
            _activeSteps.Remove(step);
            _analytics.SendTutorialStepCompleted(Id, step.Order);

            if (_activeSteps.Count == 0 && _steps.Count > 0)
                NextStepCluster();
            else if(_steps.Count == 0)
                Complete();
        }

        private void Complete()
        {
            _isRunning = false;
            OnComplete?.Invoke(this);
        }

        private void CreateTrigger()
        {
            switch (_config.TriggerType)
            {
                case TutorialTriggerType.None:
                    _trigger = new NoneTrigger();
                    break;
                case TutorialTriggerType.QuestStart:
                    _trigger = new QuestStartTrigger(_questService, _config.TriggerJSON);
                    break;
                case TutorialTriggerType.QuestComplete:
                    _trigger = new QuestCompleteTrigger(_questService, _config.TriggerJSON);
                    break;
                case TutorialTriggerType.QuestCollect:
                    _trigger = new QuestCollectTrigger(_questService, _config.TriggerJSON);
                    break;
                case TutorialTriggerType.MainModeEnter:
                    _trigger = new MainModeEnterTrigger(_mainMode, _config.TriggerJSON);
                    break;
                case TutorialTriggerType.Button:
                    _trigger = new ButtonTutorialTrigger(_buttonService, _config.TriggerJSON);
                    break;
                case TutorialTriggerType.MainModeBoseLose:
                    _trigger = new MainModeBossLoseTrigger(_mainMode);
                    break;
                case TutorialTriggerType.ContentUnlock:
                    _trigger = new ContentUnlockTutorialTrigger(_contentService, _config.TriggerJSON);
                    break;
                case TutorialTriggerType.ContentReadyManualUnlock:
                    _trigger = new ContentManualTutorialTrigger(_contentService, _config.TriggerJSON);
                    break;
            }
            
            _trigger.OnTriggered += OnTriggered;
            _trigger.Initialize();
        }

        private void OnTriggered()
        {
            DestroyTrigger();
            ReadyToStart();
        }

        public void DestroyTrigger()
        {
            if(_trigger == null) return;
            _trigger.OnTriggered -= OnTriggered;
            _trigger.Dispose();
            _trigger = null;
        }

        private void ReadyToStart() => 
            OnReadyToStart?.Invoke(this);
    }
}