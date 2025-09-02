using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Factories;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Model;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Services;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.View;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Service;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Views
{
    public class PiggyBankWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField, Required] private PiggyBankUIElementsContainer _uiContainer;
        
        [Inject] private PiggyBankService _piggyBankService;
        [Inject] private ITranslator _translator;
        [Inject] private PiggyBankBonusService _bonusService;
        [Inject] private PiggyBankBonusViewFactory _bonusViewFactory;
        [Inject] private IWindowService _windows;

        private const string LevelHintPrefixToken = "piggy_bank_lvl_hint_prefix";
        private const string LevelHintSuffixToken = "piggy_bank_lvl_hint_suffix";
        private const string LimitHintToken = "piggy_bank_limit_hint";
        private const string LevelPrefixToken = "piggy_bank_level_prefix";
        private const string LevelSuffixToken = "piggy_bank_level_suffix";
        private const string MaxLevelToken = "Max";
        
        private const float SliderTweenTime = 0.3f;
        private const float DelayBeforeNewBatch = 0.5f;
        private const int BonusesPerBatch = 5;
        
        private int _currentBonusBatchIndex = 0;
        private List<PiggyBankBonusView> _bonusViews = new List<PiggyBankBonusView>();
        private Tween _bonusTween;

        private void Start()
        {
            _uiContainer.BuyButton.OnClick += TryCollect;
            _piggyBankService.OnChanged += RedrawAlways;
            _piggyBankService.OnCollected += RedrawOnCollect;
            _piggyBankService.OnCollected += CheckBonuses;
            _uiContainer.GuideButton.OnClick += ShowGuide;
            RedrawOnStart();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _uiContainer.BuyButton.OnClick -= TryCollect;
            _piggyBankService.OnChanged -= RedrawAlways;
            _piggyBankService.OnCollected -= RedrawOnCollect;
            _piggyBankService.OnCollected -= CheckBonuses;
            _uiContainer.GuideButton.OnClick -= ShowGuide;
            ClearBonusViews();
        }

        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        private void TryCollect()
        {
            _piggyBankService.Collect(_uiContainer.CurrentHardText.transform.position).Forget();
        }

        private void ShowGuide()
        {
            _windows.OpenAsync<PiggyBankGuideWindow>();
        }

        private void RedrawOnStart()
        {
            RedrawAlways();
            RedrawOnCollect();
            
            int collectedCount = _bonusService.Bonuses.Count(b => b.IsCollected);
            _currentBonusBatchIndex = collectedCount / BonusesPerBatch;
            
            DrawBonusBatch();
            
            int collectedInBatch = _bonusViews.Count(b => b.IsCollected);
            float targetValue = (float)(collectedInBatch - 1) / (BonusesPerBatch - 1);
            _uiContainer.BonusSlider.value = targetValue;
        }

        private void RedrawAlways()
        {
            _uiContainer.ProgressSlider.minValue = 0;
            _uiContainer.ProgressSlider.maxValue = _piggyBankService.GetHardLimit();
            _uiContainer.CurrentHardText.text = _piggyBankService.CurrentHard.ToString();
            _uiContainer.ProgressSlider.value = _piggyBankService.CurrentHard;
            
            //TODO: убрал для тестов
            //_uiContainer.BuyButton.SetInteractable(_piggyBankService.CurrentHard >= _piggyBankService.GetHardLimit() / 2);
        }

        private void RedrawOnCollect()
        {
            _uiContainer.ProgressHardLimitText.text = _piggyBankService.GetHardLimit().ToString();
            _uiContainer.ProgressHalfHardLimitText.text = (_piggyBankService.GetHardLimit() / 2).ToString();
            _uiContainer.NextHardLimitText.text = _piggyBankService.GetNextHardLimit().ToString();
            _uiContainer.Icon.sprite = _piggyBankService.GetSprite();
            _uiContainer.LevelText.text = $"{_translator.Translate(LevelPrefixToken)}{_piggyBankService.Level}{_translator.Translate(LevelSuffixToken)}";
            
            string levelHint = $"{_translator.Translate(LevelHintPrefixToken)}" +
                               $"{_piggyBankService.Level}{_translator.Translate(LevelHintSuffixToken)}";
            _uiContainer.LevelHintText.text = levelHint;
            
            string limitHint = $"{_translator.Translate(LimitHintToken)}{_piggyBankService.GetHardLimit()}";
            _uiContainer.HardLimitHintText.text = limitHint;
            
            if (_piggyBankService.IsMaxLevel())
            {
                _uiContainer.NextLevelText.text = _translator.Translate(MaxLevelToken);
                _uiContainer.LevelArrow.gameObject.Off();
                _uiContainer.NextLevelImage.InvalidColor();
                _uiContainer.NextLimitImage.InvalidColor();
            }
            else
            {
                _uiContainer.NextLevelText.text = (_piggyBankService.Level + 1).ToString();
                _uiContainer.LevelArrow.gameObject.On();
                _uiContainer.NextLevelImage.ValidColor();
                _uiContainer.NextLimitImage.ValidColor();
            }

            _uiContainer.PriceText.text = _piggyBankService.GetPrice();
        }

        private void CheckBonuses()
        {
            PiggyBankBonusView targetBonusView = _bonusViews.FirstOrDefault(b => !b.IsCollected);

            if (targetBonusView != null && targetBonusView.NeedLevel <= _piggyBankService.Level)
            {
                float vfxDelay = _bonusViews.Count(b => b.IsCollected) == 0 ? 0 : SliderTweenTime;
                targetBonusView.Collect(targetBonusView.transform.position, vfxDelay);
                int collectedInBatch = _bonusViews.Count(b => b.IsCollected);
                
                if (collectedInBatch == 1)
                {
                    targetBonusView.UpdateView();
                    targetBonusView.Punch();
                    return;
                }
                
                float targetValue = (float)(collectedInBatch - 1) / (BonusesPerBatch - 1);
                AnimateBonusProgress(targetValue, () =>
                {
                    targetBonusView.UpdateView();
                    targetBonusView.Punch();

                    if (_bonusViews.All(b => b.IsCollected))
                    {
                        Tween.Delay(DelayBeforeNewBatch, () =>
                        {
                            _currentBonusBatchIndex++;
                            DrawBonusBatch();
                        });
                    }
                });
            }
        }
        
        private void AnimateBonusProgress(float targetValue, Action onComplete)
        {
            if (_bonusTween.isAlive)
                _bonusTween.Complete();

            _bonusTween = Tween.Custom(
                startValue: _uiContainer.BonusSlider.value,
                endValue: targetValue,
                duration: SliderTweenTime,
                onValueChange: v => _uiContainer.BonusSlider.value = v
            ).OnComplete(onComplete);
        }

        private void DrawBonusBatch()
        {
            ClearBonusViews();

            var allBonuses = _bonusService.Bonuses;
            int totalBonuses = allBonuses.Count;
            int startIndex = _currentBonusBatchIndex * BonusesPerBatch;
            int endIndex = Mathf.Min(startIndex + BonusesPerBatch, totalBonuses);

            if (startIndex >= totalBonuses)
            {
                var lastCollected = allBonuses
                    .Where(b => b.IsCollected)
                    .TakeLast(BonusesPerBatch)
                    .ToList();

                foreach (var bonus in lastCollected)
                    CreateBonusView(bonus);

                _uiContainer.BonusSlider.value = _uiContainer.BonusSlider.maxValue;
                return;
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                var bonus = allBonuses[i];
                CreateBonusView(bonus);
            }
            
            _uiContainer.BonusSlider.value = 0f;
        }

        private void ClearBonusViews()
        {
            foreach (PiggyBankBonusView view in _bonusViews)
                _bonusViewFactory.Release(view);
            
            _bonusViews.Clear();
        }

        private void CreateBonusView(BasePiggyBankBonus bonus)
        {
            var view = _bonusViewFactory.Create();
            view.Setup(bonus);
            view.transform.SetParent(_uiContainer.BonusesParent);
            view.transform.localScale = Vector3.one;
            view.transform.localPosition = Vector3.zero;
            _bonusViews.Add(view);
        }
    }
}