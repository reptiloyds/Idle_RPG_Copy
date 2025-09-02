using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.View;
using PleasantlyGames.RPG.Runtime.Core.GuideWindows;
using PleasantlyGames.RPG.Runtime.Core.Roulette.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.SlotMachine;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SlotRushWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private SlotMachineView _slotMachine;
        [SerializeField] private SubModeEnterButtons _enterButtons;
        [SerializeField] private SubModeEnterResourceView _enterResourceView;
        [SerializeField] private BaseButton _guideButton;

        [Inject] private SlotRushRoulette _slotRushRoulette;
        [Inject] private IWindowService _windowService;
        
        protected override void Awake()
        {
            base.Awake();
            
            _enterButtons.OnClicked += Spin;
            _enterButtons.Setup(_slotRushRoulette, false,
                () => !_slotMachine.IsSpinning);
            
            _slotMachine.Initialize(_slotRushRoulette.SlotDataList);
            _slotMachine.OnStop += OnStopSpinning;
            _guideButton.OnClick += ShowGuide;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _enterButtons.OnClicked -= Spin;
            _slotMachine.OnStop -= OnStopSpinning;
            _guideButton.OnClick -= ShowGuide;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        private async void OnStopSpinning(SlotData slotData, SlotView slotView)
        {
            var window = await _windowService.OpenAsync<RouletteRewardWindow>();
            window.Setup(slotView.Sprite, slotView.Text);
            window.OnContinue += OnContinue;
            _enterButtons.Redraw();
        }

        //TODO REWORK APPLY RESULT
        private void OnContinue(RouletteRewardWindow window)
        {
            window.OnContinue -= OnContinue;
            _slotRushRoulette.ApplyReward(_slotMachine.Result.Index, window.ImageRect);
        }

        protected override void OnCloseClick()
        {
            if(_slotMachine.IsSpinning) return;
            base.OnCloseClick();
        }

        private void ShowGuide()
        {
            _windowService.OpenAsync<SlotRushGuideWindow>();
        }

        protected virtual void Spin()
        {
            _slotRushRoulette.Spin();
            _slotMachine.Spin();
        }

        public override void Open()
        {
            base.Open();
            
            _slotRushRoulette.OnBonusEnterSpent += RedrawDynamic;
            _slotRushRoulette.EnterResource.OnChange += RedrawDynamic;
            RedrawDynamic();
        }

        public override void Close()
        {
            if (_slotRushRoulette != null)
            {
                _slotRushRoulette.OnBonusEnterSpent -= RedrawDynamic;
                if(_slotRushRoulette.EnterResource != null)
                    _slotRushRoulette.EnterResource.OnChange -= RedrawDynamic;   
            }
            base.Close();
        }

        private void RedrawDynamic()
        {
            _enterButtons.Redraw();
            _enterResourceView.Redraw(_slotRushRoulette);
        }
    }
}