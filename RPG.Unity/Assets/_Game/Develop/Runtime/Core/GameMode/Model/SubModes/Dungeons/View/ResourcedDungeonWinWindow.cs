using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Contract;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PrimeTween;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View
{
    public abstract class ResourcedDungeonWinWindow<T> : BaseWindow where T : DungeonMode, IResourcefulDungeon
    {
        [SerializeField] private List<UITimer> _autoContinueTimers;
        [SerializeField] private GameObject _autoSweepObject;
        
        [SerializeField] private DungeonRewardPresenter _rewardPresenter;
        [SerializeField] private BaseButton _okButton;
        [SerializeField] private SubModeEnterButtons _enterButtons;
        [SerializeField] private SubModeEnterResourceView _enterResourceView;

        [Inject] protected T Dungeon;
        [Inject] protected DungeonModeFacade ModeFacade;
        [Inject] private IPopupResourceFactory _factory;
        [Inject] private ResourceViewService _resourceViewService;
        [Inject] private DungeonConfiguration _configuration;
        [Inject] private TimeService _time;
        
        private readonly SerialDisposable _autoSweepTimer = new();
        private readonly ReactiveProperty<float> _autoSweepDelay = new();

        private int _completedLevel;
        
        protected override void Awake()
        {
            base.Awake();
            
            _okButton.OnClick += OnOkClick;
            _enterButtons.OnEnter += OnEnter;
            _enterButtons.OnBonusEnter += OnBonusEnter;
            _enterButtons.Setup(Dungeon, false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _okButton.OnClick -= OnOkClick;
            _enterButtons.OnEnter += OnEnter;
            _enterButtons.OnBonusEnter += OnBonusEnter;
        }

        public override void Open()
        {
            _completedLevel = Dungeon.AvailableLevel - 1;
            _rewardPresenter.SpawnReward(Dungeon.RewardImage, Dungeon.GetRewardFor(_completedLevel), Dungeon.RewardColor);
            
            _enterResourceView.Redraw(Dungeon);
            if(Dungeon.IsAutoSweep && (Dungeon.IsEnterResourceEnough || Dungeon.BonusEnterAmount > 0))
                EnableAutoSweep();
            else
                DisableAutoSweep();

            _enterButtons.Redraw();
            
            base.Open();
        }

        public override void Close()
        {
            _autoSweepTimer.Disposable?.Dispose();
            foreach (var timer in _autoContinueTimers) 
                timer.Stop();
            base.Close();
        }

        private void EnableAutoSweep()
        {
            _enterButtons.gameObject.SetActive(true);
            _autoSweepDelay.Value = _configuration.AutoSweepDelay;
            _time.LaunchLocalTimer(_autoSweepTimer, _autoSweepDelay, OnAutoSweepTimerEnd);
            foreach (var timer in _autoContinueTimers) 
                timer.Listen(_autoSweepDelay);
            _autoSweepObject.SetActive(true);
        }

        private void DisableAutoSweep()
        {
            _enterButtons.gameObject.SetActive(false);
            _autoSweepObject.SetActive(false);
        }

        private void OnOkClick()
        {
            ApplyReward();
            ModeFacade.Exit();
            Close();
        }

        private void ApplyReward()
        {
            var target = _resourceViewService.GetView(Dungeon.RewardType);
            var amount = Dungeon.GetRewardFor(_completedLevel);
            _factory.SpawnFromUI(_rewardPresenter.RewardPoint, Dungeon.RewardImage, amount, 6,
                target.transform, target, true, PopupIconEffect.None, PopupIconContext.OverUI);
            Dungeon.ApplyRewardFor(_completedLevel);
        }

        private void OnAutoSweepTimerEnd()
        {
            if (!Dungeon.IsEnterResourceEnough) return;
            ApplyReward();
            Close();
            Dungeon.SetLaunchLevel(Dungeon.LaunchLevel + 1);
            ModeFacade.Next(Dungeon);
        }

        private void OnEnter()
        {
            Next();
            ModeFacade.Next(Dungeon);
        }

        private void OnBonusEnter()
        {
            Next();
            ModeFacade.BonusLaunch(Dungeon);
        }

        private void Next()
        {
            ApplyReward();
            Close();
            Dungeon.SetLaunchLevel(Dungeon.LaunchLevel + 1);
        }
    }
}