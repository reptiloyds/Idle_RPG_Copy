using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.View;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Tween;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class QuestView : MonoBehaviour, IInitializable
    {
        [SerializeField, Required] private GameObject _viewObject;
        [SerializeField, Required] private BaseButton _button;
        [SerializeField, Required] private GameObject _defaultBackground;
        [SerializeField, Required] private GameObject _completeBackground;
        [SerializeField, Required] private TextMeshProUGUI _infoText;
        [SerializeField, Required] private TextMeshProUGUI _descriptionText;
        [SerializeField, Required] private Image _rewardImage;
        [SerializeField, Required] private TextMeshProUGUI _rewardText;
        [SerializeField, Required] private UIScale _uiScale;
        [SerializeField, Required] private HintPointerView _hintPointerView;
        [SerializeField] private bool _showTextProgression = true;
        [SerializeField] private bool _updateProgressBar = false;
        [SerializeField, HideIf("@this._updateProgressBar == false")] private Slider _progressBar;
        [SerializeField, HideIf("@this._updateProgressBar == false")] private TextMeshProUGUI _progressText;
        
        private bool _isActive;
        private readonly int _maxViewReward = 5;

        [Inject] private ITranslator _translator;
        [Inject] private QuestService _service;
        [Inject] private MainMode _mainMode;
        [Inject] private TutorialService _tutorialService;

        void IInitializable.Initialize()
        {
            // if(_mainMode.IsLaunched)
            //     Enable();
            // else
            //     Disable();
            
            _service.OnQuestStart += OnQuestStart;
            _service.OnQuestComplete += OnQuestComplete;
            _service.OnProgressChanged += OnProgressChanged;
            _service.OnQuestDisposed += OnQuestDisposed;

            if (_service.Quest != null)
                EnableQuest();
            else
                DisableQuest();
            
            _mainMode.OnLaunched += OnMainModeLaunched;
            _mainMode.OnDisposed += OnMainModeDisposed;
            
            _button.OnClick += OnClick;
            _tutorialService.OnTutorialStarted += OnTutorialStateChange;
            _tutorialService.OnTutorialCompleted += OnTutorialStateChange;
        }

        private void OnDestroy()
        {
            _service.OnQuestStart -= OnQuestStart;
            _service.OnQuestComplete -= OnQuestComplete;
            _service.OnProgressChanged -= OnProgressChanged;
            _service.OnQuestDisposed -= OnQuestDisposed;
            _mainMode.OnLaunched -= OnMainModeLaunched;
            _mainMode.OnDisposed -= OnMainModeDisposed;
            _button.OnClick -= OnClick;
            _tutorialService.OnTutorialStarted -= OnTutorialStateChange;
            _tutorialService.OnTutorialCompleted -= OnTutorialStateChange;
        }

        private void OnTutorialStateChange()
        {
            if (_tutorialService.HasActiveTutorial)
                _hintPointerView.gameObject.SetActive(false);
            else
                _hintPointerView.gameObject.SetActive(true);
        }

        private void OnProgressChanged() => RedrawDynamic();
        private void OnQuestComplete(int questId) => RedrawState();
        private void OnQuestStart(int questId) => EnableQuest();
        private void OnQuestDisposed(int questId) => DisableQuest();
        
        private void OnMainModeLaunched(IGameMode gameMode)
        {
            if(_isActive) return;
            Enable();
        }

        private void OnMainModeDisposed(IGameMode gameMode)
        {
            if(!_isActive) return;
            Disable();
        }

        private void OnClick()
        {
            if (!_service.IsCompleteCurrent) return;
            _service.ConfirmReward(_rewardImage.transform.position);
        }

        private void Enable()
        {
            gameObject.SetActive(true);
            _isActive = true;
        }

        private void Disable()
        {
            gameObject.SetActive(false);
            _isActive = false;
        }

        private void EnableQuest()
        {
            _viewObject.SetActive(true);
            RedrawState();
            RedrawDynamic();
            _rewardImage.sprite = _service.Quest.RewardSprite;
            _rewardText.SetText(StringExtension.Instance.CutDouble(_service.Quest.RewardAmount, true));
        }
        
        private void DisableQuest() => 
            _viewObject.SetActive(false);

        private void RedrawDynamic()
        {
            var progressTuple = _service.Quest.GetProgress();
            if (_showTextProgression && !string.IsNullOrEmpty(progressTuple.progressText)) 
                _descriptionText.SetText($"{_service.Quest.GetDescription()} {progressTuple.progressText}");
            else
                _descriptionText.SetText(_service.Quest.GetDescription());

            if (_updateProgressBar)
            {
                _progressBar.value = progressTuple.progress;
                _progressText.SetText(progressTuple.progressText);
            }
        }

        private void RedrawState()
        {
            _defaultBackground.SetActive(!_service.IsCompleteCurrent);
            _completeBackground.SetActive(_service.IsCompleteCurrent);
            
            if (_service.IsCompleteCurrent)
            {
                _infoText.SetText(_translator.Translate(TranslationConst.CollectReward));
                _uiScale.Play();
                _button.SetInteractable(true);
                _hintPointerView.Ready();
            }
            else
            {
                _infoText.SetText($"{_translator.Translate(TranslationConst.Quest)} {_service.QuestId}");
                _uiScale.Stop();
                _button.SetInteractable(false);
                _hintPointerView.NotReady();
            }
        }
    }
}