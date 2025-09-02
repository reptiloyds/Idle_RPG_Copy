using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.BonusAccess.View;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Deal.Controller;
using PleasantlyGames.RPG.Runtime.Core.Deal.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.View.Chance;
using PleasantlyGames.RPG.Runtime.Core.PurchasePresentation;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PrimeTween;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class LootboxView : MonoBehaviour
    {
        [SerializeField, Required] private LevelProgressionView _levelProgressionView;
        [SerializeField, Required] private PurchaseContentPresenter _contentPresenter;
        [SerializeField, Required] private BaseButton _infoButton;
        [SerializeField, Required] private TextMeshProUGUI _nameText;
        [SerializeField] private List<ResourceDealView> _resourceDealViews;
        [SerializeField, Required] private GameObject _blockObject;
        [SerializeField, Required] private TextMeshProUGUI _unlockConditionText;
        
        [SerializeField, FoldoutGroup("LevelUp")] private TweenSettings<float> _showLevelUpSettings;
        [SerializeField, FoldoutGroup("LevelUp")] private TweenSettings<float> _hideLevelUpSettings;
        [SerializeField, FoldoutGroup("LevelUp")] private CanvasGroup _levelUp;
        [SerializeField, FoldoutGroup("LevelUp")] private TextMeshProUGUI _previousLevelText;
        [SerializeField, FoldoutGroup("LevelUp")] private TextMeshProUGUI _currentLevelText;
        
        [SerializeField] private BonusAccessButton _bonusButton;

        [Inject] private ITranslator _translator;
        [Inject] private ResourceService _resourceService;
        [Inject] private MessageBuffer _messageBuffer;
        [Inject] private IWindowService _windowService;
        private LootboxOpeningWindow _lootboxOpeningWindow;

        private readonly Dictionary<ResourceDealController, int> _dealAmount = new();
        private Sequence _levelUpSequence;
        private Lootbox _model;
        private int _previousLevel;
        
        private bool _bonusIsEnabled;

        public Lootbox Model => _model;
        public BaseButton BonusButton => _bonusButton;
        public event Action<LootboxView, int> OnOpenClicked;
        public event Action<LootboxView> OnBonusClicked;

        private async void Awake()
        {
            _lootboxOpeningWindow = await _windowService.GetAsync<LootboxOpeningWindow>(false);
            _lootboxOpeningWindow.OnClosed += OnLootboxOpeningClosed;
            HideLevelUp();
        }

        private void OnDestroy()
        {
            foreach (var pair in _dealAmount) 
                pair.Key.OnSuccess -= OnDealSuccess;
            _levelUpSequence.Stop();
            _dealAmount.Clear();
            _model.OnUnlocked -= OnUnlocked;
            _model.OnItemsApplied -= OnItemsApplied;
            _model = null;
            _infoButton.OnClick -= OnInfoButtonClick;
            _lootboxOpeningWindow.OnClosed -= OnLootboxOpeningClosed;
        }

        private void OnLootboxOpeningClosed(BaseWindow window) => 
            RedrawLevelView();

        private void OnEnable()
        {
            if (_bonusIsEnabled) 
                RedrawBonusButton();
        }

        private void OnDisable() => 
            HideLevelUp();

        public void Setup(Lootbox lootbox)
        {
            for (int i = 0; i < _resourceDealViews.Count; i++)
            {
                var buttonNumber = i + 1;
                var view = _resourceDealViews[i];
                view.Button.ChangeButtonId($"{lootbox.Id}_Buy_{buttonNumber}");
            }

            _model = lootbox;
            _model.OnUnlocked += OnUnlocked;
            _infoButton.OnClick += OnInfoButtonClick;

            if (_model.IsUnlocked)
                _blockObject.SetActive(false);
            else
            {
                _unlockConditionText.SetText(_model.Condition);
                _blockObject.SetActive(true);
            }

            if (_model.HasBonusOpen)
            {
                _bonusButton
                    .SetCooldown(_model.BonusOpenHandler.Cooldown)
                    .SetMainAction(OnBonusClick)
                    .SetUsages(_model.BonusOpenHandler.OpenAmount)
                    .Build();
                _model.OnItemsApplied += OnItemsApplied;
                RedrawBonusButton();
                _bonusIsEnabled = true;
            }
            else
                _bonusButton.gameObject.SetActive(false);

            _contentPresenter.Redraw(_model.Sprites, _model.Color, PurchaseContentBackground.Glow);

            for (int i = 0; i < _resourceDealViews.Count; i++)
            {
                var dealView = _resourceDealViews[i];
                if (i >= _model.PurchaseDataList.Count)
                    dealView.gameObject.SetActive(false);
                else
                {
                    var purchaseData = _model.PurchaseDataList[i];
                    dealView.gameObject.SetActive(true);
                    dealView.SetLabelText($"{_translator.Translate(TranslationConst.Buy)} X{purchaseData.ItemAmount}");
                    var dealController =
                        new ResourceDealController(dealView, _resourceService, _messageBuffer, _translator);
                    dealController
                        .AddPrice(purchaseData.ResourceType, purchaseData.Price)
                        .BuildPrice();

                    _dealAmount.Add(dealController, purchaseData.ItemAmount);
                    dealController.OnSuccess += OnDealSuccess;
                }
            }

            _nameText.SetText(_translator.Translate(_model.Id));
            RedrawLevelView();
        }

        private void OnUnlocked(IUnlockable unlockable) => 
            _blockObject.SetActive(false);

        private async void OnInfoButtonClick()
        {
            var window = await _windowService.OpenAsync<LootboxChanceWindow>();
            window.Setup(_model.Level, _model.Id);
        }

        private void OnBonusClick() => 
            OnBonusClicked?.Invoke(this);

        private void OnDealSuccess(ResourceDealController dealController)
        {
            var amount = _dealAmount[dealController];
            OnOpenClicked?.Invoke(this, amount);
        }

        private void RedrawLevelView()
        {
            if (_previousLevel == 0)
            {
                _previousLevel = _model.Level;
                _levelProgressionView.ZeroFill();
            }
            else if (_model.Level > _previousLevel)
            {
                ShowLevelUp();
                _previousLevel = _model.Level;
                _levelProgressionView.ZeroFill();  
            }
            _levelProgressionView.Redraw(_model.Level, _model.IsLevelMax, _model.Experience,
                _model.TargetExperience, 0.5f);
        }

        private void ShowLevelUp()
        {
            _levelUp.gameObject.SetActive(true);
            _previousLevelText.SetText(_previousLevel.ToString());
            _currentLevelText.SetText(_model.Level.ToString());
            _levelUp.alpha = 0;
            _levelUpSequence.Stop();
            _levelUpSequence = Sequence.Create(useUnscaledTime: true);
            _levelUpSequence.Chain(Tween.Alpha(_levelUp, _showLevelUpSettings));
            _levelUpSequence.Chain(Tween.Alpha(_levelUp, _hideLevelUpSettings));
            _levelUpSequence.OnComplete(HideLevelUp);
        }

        private void HideLevelUp()
        {
            _levelUpSequence.Stop();
            _levelUp.gameObject.SetActive(false);
        }

        private void OnItemsApplied(Lootbox lootbox, IReadOnlyList<Item> readOnlyList) => 
            RedrawBonusButton();

        private void RedrawBonusButton()
        {
            _bonusButton
                .SetLabelText($"{_translator.Translate(TranslationConst.Receive)} X{_model.BonusOpenHandler.Items}")
                .SetAdditionalText($"({_model.BonusOpenHandler.OpenAmount}/{_model.BonusOpenHandler.MaxOpenAmount})");
            _bonusButton.UpdateState();
        }
    }
}