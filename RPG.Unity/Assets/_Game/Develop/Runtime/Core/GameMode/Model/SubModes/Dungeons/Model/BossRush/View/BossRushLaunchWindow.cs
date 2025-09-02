using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components;
using PleasantlyGames.RPG.Runtime.Core.GuideWindows;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.View
{
    public class BossRushLaunchWindow : DungeonLaunchWindow<BossRushMode>
    {
        [SerializeField, BoxGroup("Animation")]
        private RectTransform[] _animationTargets;

        [SerializeField] private SubModeRewardPreview _rewardPreview;
        [SerializeField] private DungeonLevelSelector _levelSelector;
        [SerializeField] private DungeonRewardView _rewardView;
        [SerializeField] private DungeonAutoSweepView _autoSweepView;
        [SerializeField] private BaseButton _sweepButton;
        [SerializeField] private BaseButton _guideButton;

        protected override void Awake()
        {
            base.Awake();
            _levelSelector.OnCurrentLevelChanged += OnCurrentLevelChanged;
            _autoSweepView.OnToggled += OnAutoSweepToggled;
            _sweepButton.OnClick += OnSweepClick;
            _guideButton.OnClick += ShowGuide;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _levelSelector.OnCurrentLevelChanged -= OnCurrentLevelChanged;
            _autoSweepView.OnToggled -= OnAutoSweepToggled;
            _sweepButton.OnClick -= OnSweepClick;
            _guideButton.OnClick -= ShowGuide;
        }

        protected override void OpenAnimation(Action callback) =>
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) =>
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        private async void OnSweepClick()
        {
            var window = await WindowService.OpenAsync<DungeonSweepWindow>();
            window.Setup(Dungeon, _levelSelector.CurrentLevel);
        }

        protected override void OnEnter()
        {
            Dungeon.SetLaunchLevel(_levelSelector.CurrentLevel);
            base.OnEnter();
        }

        protected override void OnBonusEnter()
        {
            Dungeon.SetLaunchLevel(_levelSelector.CurrentLevel);
            base.OnBonusEnter();
        }

        private void ShowGuide()
        {
            WindowService.OpenAsync<BossRushGuideWindow>();
        }

        private void OnAutoSweepToggled() =>
            Dungeon.SetAutoSweep(_autoSweepView.IsOn);

        public override void Open()
        {
            _autoSweepView.SetValue(Dungeon.IsAutoSweep);
            _levelSelector.Setup(Dungeon.AvailableLevel, Dungeon.MaxLevel);
            _rewardPreview.Redraw(Dungeon);
            base.Open();
        }

        private void OnCurrentLevelChanged() =>
            RedrawDynamic();

        protected override void RedrawDynamic()
        {
            base.RedrawDynamic();
            _sweepButton.SetInteractable(!_levelSelector.IsLevelAvailable && Dungeon.IsEnterResourceEnough);
            _rewardView.Redraw(Dungeon.RewardImage, Dungeon.GetRewardFor(_levelSelector.CurrentLevel));
        }
    }
}