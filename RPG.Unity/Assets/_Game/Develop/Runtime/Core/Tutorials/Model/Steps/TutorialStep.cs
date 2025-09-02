using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Button;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Phone;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Quest;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Timer;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps
{
    internal abstract class TutorialStep
    {
        [Inject] private IPauseService _pauseService;
        [Inject] private IButtonService _buttonService;
        [Inject] private QuestService _questService;
        [Inject] protected IWindowService WindowService;
        [Inject] private ChatService _chatService;
        
        protected TutorialCondition Condition { get; private set; }
        private bool _isConstrainsEnabled;

        private readonly TutorialElem _config;
        private bool IsPopupShouldBlocked => _config.ForcePopupBlock || HasAnyConstraint;
        private bool HasAnyConstraint => _config.PauseTime || _config.PauseSave || _config.AllowedButtonsList.Count > 0;
        
        public List<string> AllowedButtons => _config.AllowedButtonsList;
        public float StartDelay => _config.StartDelay;
        public int Order => _config.Order;
        public event Action<TutorialStep> Completed;

        protected TutorialStep(TutorialElem config) => 
            _config = config;

        public void PreStart(bool isFirstStep, bool isLaunchStep)
        {
            if(!isFirstStep)
                EnableConstraints();
            if(isLaunchStep)
                CloseWindows();
        }

        public virtual void Start()
        {
            _buttonService.AppendAllowedButtonIds(_config.AllowedButtonsList);
            EnableConstraints();
            CreateCompleteCondition();
        }

        public virtual void Dispose()
        {
            _buttonService.RemoveAllowedButtonIds(_config.AllowedButtonsList);
            DisableConstraints();
            ClearCondition();
        }

        public virtual void Continue()
        {
            _buttonService.AppendAllowedButtonIds(_config.AllowedButtonsList);
            EnableConstraints();
            Condition?.Continue();
        }

        public virtual void Pause()
        {
            _buttonService.RemoveAllowedButtonIds(_config.AllowedButtonsList);
            DisableConstraints();
            Condition?.Pause();
        }

        private void CloseWindows()
        {
            if (_config.CloseWindowData.Close)
            {
                if (_config.CloseWindowData.ExcludeIds.Count > 0)
                    WindowService.CloseAll(_config.CloseWindowData.ExcludeIds);
                else if (_config.CloseWindowData.Ids.Count > 0)
                    foreach (var id in _config.CloseWindowData.Ids) 
                        WindowService.Close(id);
                else
                    WindowService.CloseAll();
            }
        }

        private void EnableConstraints()
        {
            if(_isConstrainsEnabled) return;
            _isConstrainsEnabled = true;

            if (IsPopupShouldBlocked) 
                WindowService.BlockPopupPool();
            
            if(_config.PauseTime)
                _pauseService.Pause(PauseType.Time);
            if(_config.AllowedButtonsList.Count > 0)
                _pauseService.Pause(PauseType.UIInput);
            if(_config.PauseSave)
                _pauseService.Pause(PauseType.Save);
        }

        private void DisableConstraints()
        {
            if(!_isConstrainsEnabled) return;
            _isConstrainsEnabled = false;
            
            if (IsPopupShouldBlocked) 
                WindowService.UnlockPopupPool();
            
            if(_config.PauseTime)
                _pauseService.Continue(PauseType.Time);
            if(_config.AllowedButtonsList.Count > 0)
                _pauseService.Continue(PauseType.UIInput);
            if(_config.PauseSave)
                _pauseService.Continue(PauseType.Save);
        }

        private void CreateCompleteCondition()
        {
            switch (_config.CompleteCondition)
            {
                case TutorialCompleteCondition.None:
                    return;
                case TutorialCompleteCondition.Button:
                    Condition = new ButtonTutorialCondition(_buttonService, _config.ConditionJSON);
                    break;
                case TutorialCompleteCondition.QuestComplete:
                    Condition = new QuestCompleteCondition(_questService, _config.ConditionJSON);
                    break;
                case TutorialCompleteCondition.Timer:
                    Condition = new TimerCompleteCondition(_config.ConditionJSON);
                    break;
                case TutorialCompleteCondition.ChatComplete:
                    Condition = new ChatCompleteCondition(_chatService, _config.ConditionJSON);
                    break;
                case TutorialCompleteCondition.ShowChatAnswers:
                    Condition = new ChatAnswersShowCondition(_chatService, _config.ConditionJSON);
                    break;
            }
            
            Condition.OnComplete += Complete;
            Condition.Initialize();
        }

        protected void Complete()
        {
            ClearCondition();
            Completed?.Invoke(this);
        }

        private void ClearCondition()
        {
            if(Condition == null) return;
            
            Condition.OnComplete -= Complete;
            Condition.Dispose();
            Condition = null;
        }
    }
}