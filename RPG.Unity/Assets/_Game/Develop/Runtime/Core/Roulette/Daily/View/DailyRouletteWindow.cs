using System;
using PleasantlyGames.RPG.Runtime.BonusAccess.View;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.View;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.RouletteWheel;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class DailyRouletteWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private RouletteWheelView _rouletteView;
        [SerializeField] private BonusAccessButton _bonusButton;
        [SerializeField] private TextMeshProUGUI _remainingSpinText; 

        [Inject] private DailyRoulette _model;
        [Inject] private IWindowService _windowService;
        [Inject] private ITranslator _translator;
        [Inject] private TooltipFactory _tooltipFactory;

        private readonly CompositeDisposable _compositeDisposable = new();
        private int _rewardIndex;
        public BaseButton SpinButton => _bonusButton;

        protected override void Awake()
        {
            base.Awake();
            
            _bonusButton
                .SetLabelText(_translator.Translate(TranslationConst.Spin))
                .SetUsages(_model.SpinAmount)
                .SetFreeAccess(_model.FreeSpinAmount)
                .SetExecuteCondition(() => !_rouletteView.IsSpinning)
                .SetCooldown(_model.Cooldown)
                .Build();
            _bonusButton.OnExecuted += Executed;
            _rouletteView.OnStop += OnRouletteStop;

            _rouletteView.Initialize(_model.PieceDataList, _model.PieceDictionary);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _rouletteView.OnStop -= OnRouletteStop;
        }

        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void OnCloseClick()
        {
            if(_rouletteView.IsSpinning) return;
            base.OnCloseClick();
        }

        public override void Open()
        {
            base.Open();

            RedrawSpinAmount();
        }

        public override void Close()
        {
            base.Close();

            if (_rouletteView.IsSpinning) 
                _rouletteView.Cancel();
        }

        private void Executed() => 
            Spin();

        private void Spin()
        {
            _rewardIndex = -1;
            _rouletteView.Spin();
            _model.Spin();
            RedrawSpinAmount();
            _bonusButton.UpdateState();
        }

        private async void OnRouletteStop(WheelPieceData data, WheelPieceView view)
        {
            _rewardIndex = data.Index;
            var window = await _windowService.OpenAsync<RouletteRewardWindow>();
            _tooltipFactory.ReleaseAll();
            window.Setup(view.Sprite, view.Text);
            window.OnContinue += OnContinue;
            _bonusButton.UpdateState();
        }

        private void OnContinue(RouletteRewardWindow window)
        {
            window.OnContinue -= OnContinue;
            _model.ApplyReward(_rewardIndex, window.ImageRect);
        }

        private void RedrawSpinAmount() => 
            _remainingSpinText.SetText(string.Format(_translator.Translate(TranslationConst.RemainingSpin), _model.TotalSpinAmount));
    }
}