using System;
using PleasantlyGames.RPG.Runtime.BonusAccess.View;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class DailyView : MonoBehaviour
    {
        [Serializable]
        private class VisualSetup
        {
            public Image Image;
            public Color ProgressColor;
            public Color CompleteColor;

            public void ApplyProgressColor() => 
                Image.color = ProgressColor;

            public void ApplyCompleteColor() => 
                Image.color = CompleteColor;
        }
        
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Color _inProgressNameColor;
        [SerializeField] private Color _completeNameColor;
        [SerializeField] private Image _stateImage;
        [SerializeField] private Sprite _questionSprite;
        [SerializeField] private Sprite _exclamationSprite;
        [SerializeField] private Sprite _toggleSprite;
        [SerializeField] private Color _inProgressStateColor;
        [SerializeField] private Color _completedStateColor;
        [SerializeField] private Color _collectedStateColor;
        [SerializeField] private VisualSetup[] _visualSetups;
        [SerializeField] private GameObject _completedObject;
        
        [SerializeField] [FoldoutGroup("Progress")] private Slider _progressBar;
        [SerializeField] [FoldoutGroup("Progress")] private Image _progressBarFillImage;
        [SerializeField] [FoldoutGroup("Progress")] private Color _inProgressFillColor;
        [SerializeField] [FoldoutGroup("Progress")] private Color _completeFillColor;
        [SerializeField] [FoldoutGroup("Progress")] private GameObject _completeProgressText;
        
        [SerializeField] [FoldoutGroup("Reward")] private Image _rewardImage;
        [SerializeField] [FoldoutGroup("Reward")] private TextMeshProUGUI _rewardText;
        [SerializeField] [FoldoutGroup("Reward")] private BaseButton _tooltipButton;
        [SerializeField] [FoldoutGroup("Reward")] private RectTransform _tooltipTarget;
        
        [SerializeField] [FoldoutGroup("Interaction")] private BonusAccessButton _bonusButton;
        [SerializeField] [FoldoutGroup("Interaction")] private GameObject _claimedObject;

        [Inject] private ITranslator _translator;
        [Inject] private TooltipFactory _tooltipFactory;
        
        public BonusAccessButton Button => _bonusButton;
        public Daily Daily { get; private set; }
        public RectTransform RewardPoint { get; private set; }

        public event Action<DailyView> OnRewarded;

        private void Awake()
        {
            _bonusButton.OnExecuted += BonusExecuted;
            RewardPoint = _rewardImage.transform as RectTransform;
            _tooltipButton.OnClick += ShowTooltip;
        }

        private void OnDestroy()
        {
            _bonusButton.OnExecuted -= BonusExecuted;
            _tooltipButton.OnClick -= ShowTooltip;
        }

        private void BonusExecuted() => 
            OnRewarded?.Invoke(this);

        private void ShowTooltip()
        {
            _tooltipFactory.ShowResourceTooltip(Daily.RewardType, _tooltipTarget);
        }

        public void Setup(Daily daily)
        {
            Daily = daily;
            _bonusButton
                .SetLabelText(_translator.Translate(TranslationConst.DailiesCollectReward))
                .SetFreeAccess(new ReactiveProperty<int>(Daily.IsBonus ? 0 : -1))
                .SetExecuteCondition(() => Daily.HasReward)
                .Build();
            RedrawStaticData();
        }

        public void OnOpen()
        {
            RedrawDynamicData();
            Daily.OnProgressChanged += OnProgressChanged;
            Daily.OnCompleted += OnCompleted;
            Daily.OnRewardCollected += OnRewardCollected;
        }

        public void OnClose()
        {
            Daily.OnProgressChanged -= OnProgressChanged;
            Daily.OnCompleted -= OnCompleted;
            Daily.OnRewardCollected -= OnRewardCollected;
        }

        private void OnProgressChanged()
        {
            _name.SetText(Daily.GetDescription());
            RedrawProgress();
        }

        private void OnRewardCollected() => 
            RedrawDynamicData();

        private void OnCompleted(Daily daily) => 
            RedrawDynamicData();

        private void RedrawStaticData()
        {
            _rewardImage.sprite = Daily.RewardSprite;
            _rewardText.text = Daily.RewardAmount.ToString();
        }

        private void RedrawDynamicData()
        {
            if (Daily.IsComplete)
            {
                if (Daily.HasReward)
                    DrawCompleteState();
                else
                    DrawCollectedState();
            }
            else
                DrawInProgressState();
            
            _bonusButton.UpdateState();
        }

        private void DrawInProgressState()
        {
            _completedObject.SetActive(false);
            
            _name.SetText(Daily.GetDescription());
            _name.color = _inProgressNameColor;
            _stateImage.color = _inProgressStateColor;
            _stateImage.sprite = _questionSprite;
            
            RedrawProgress();
            _progressBarFillImage.color = _inProgressFillColor;
            _completeProgressText.SetActive(false);
            
            foreach (var visualSetup in _visualSetups)
                visualSetup.ApplyProgressColor();
            
            _bonusButton.gameObject.SetActive(true);
            _claimedObject.SetActive(false);
        }

        private void DrawCompleteState()
        {
            _completedObject.SetActive(false);
            
            _name.SetText(Daily.GetDescription());
            _name.color = _completeNameColor;
            _stateImage.color = _completedStateColor;
            _stateImage.sprite = _exclamationSprite;
            
            RedrawProgress();
            _progressBarFillImage.color = _completeFillColor;
            _completeProgressText.SetActive(true);
            
            foreach (var visualSetup in _visualSetups)
                visualSetup.ApplyCompleteColor();
            
            _bonusButton.gameObject.SetActive(true);
            _claimedObject.SetActive(false);
        }

        private void DrawCollectedState()
        {
            _completedObject.SetActive(true);
            
            _name.SetText(Daily.GetDescription());
            _name.color = _inProgressNameColor;
            _stateImage.color = _collectedStateColor;
            _stateImage.sprite = _toggleSprite;

            RedrawProgress();
            _progressBarFillImage.color = _completeFillColor;
            _completeProgressText.SetActive(false);
            
            foreach (var visualSetup in _visualSetups)
                visualSetup.ApplyProgressColor();

            _bonusButton.gameObject.SetActive(false);
            _claimedObject.SetActive(true);
        }

        private void RedrawProgress() => 
            _progressBar.value = (float)Daily.Progress / Daily.TargetValue;
    }
}