using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Contract;
using PleasantlyGames.RPG.Runtime.Core.Resource.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View
{
    public class DungeonSweepWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField] private BaseButton _minButton;
        [SerializeField] private BaseButton _maxButton;
        [SerializeField] private BaseButton _leftButton;
        [SerializeField] private BaseButton _rightButton;
        [SerializeField] private BaseButton _sweepButton;
        [SerializeField] private Image _enterResourceImage;
        [SerializeField] private TextMeshProUGUI _enterResourceAmountText;
        [SerializeField] private TextMeshProUGUI _enterResourceSelectionText;

        [Inject] private IPopupResourceFactory _factory;
        [Inject] private ResourceViewService _resourceViewService;
        [Inject] private IWindowService _windowService;
        
        private IResourcefulDungeon _dungeon;
        private int _level;
        private BigDouble.Runtime.BigDouble _selectedAmount;
        private BigDouble.Runtime.BigDouble _maxAmount;
        
        protected override void Awake()
        {
            base.Awake();
            
            _minButton.OnClick += OnMinClick;
            _maxButton.OnClick += OnMaxClick;
            _leftButton.OnClick += OnLeftClick;
            _rightButton.OnClick += OnRightClick;
            _sweepButton.OnClick += OnSweepClick;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _minButton.OnClick -= OnMinClick;
            _maxButton.OnClick -= OnMaxClick;
            _leftButton.OnClick -= OnLeftClick;
            _rightButton.OnClick -= OnRightClick;
            _sweepButton.OnClick -= OnSweepClick;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        private async void OnSweepClick()
        {
            var totalReward = _dungeon.GetRewardFor(_level) * _selectedAmount;
            var rewardImage = _dungeon.RewardImage;
            var window = await _windowService.OpenAsync<DungeonSweepRewardWindow>();
            window.Setup(rewardImage, totalReward);
            
            var target = _resourceViewService.GetView(_dungeon.RewardType);
            _factory.SpawnFromUI(window.RewardPoint, rewardImage, totalReward, 6,
                target.transform, target, true, PopupIconEffect.None, PopupIconContext.OverUI);
            
            _dungeon.Sweep(_level, (int)_selectedAmount.ToDouble());

            Redraw();
            if(_maxAmount <= 0)
                Close();
        }

        private void OnLeftClick()
        {
            if(_selectedAmount <= 1) return;
            _selectedAmount--;
            RedrawSelectedResource();
        }

        private void OnRightClick()
        {
            if(_selectedAmount >= _maxAmount) return;
            _selectedAmount++;
            RedrawSelectedResource();
        }

        private void OnMinClick()
        {
            _selectedAmount = 1;
            RedrawSelectedResource();
        }

        private void OnMaxClick()
        {
            _selectedAmount = _maxAmount;
            RedrawSelectedResource();
        }

        private void OnCloseClick() => 
            Close();

        public void Setup(IResourcefulDungeon dungeon, int level)
        {
            _dungeon = dungeon;
            _level = level;
            Redraw();
        }
        
        private void Redraw()
        {
            _maxAmount = _dungeon.EnterResource.Value;
            _selectedAmount = _maxAmount;
            _enterResourceImage.sprite = _dungeon.EnterResource.Sprite;
            _enterResourceAmountText.SetText($"x{_maxAmount}");
            RedrawSelectedResource();
        }

        public override void Close()
        {
            _dungeon = null;
            base.Close();
        }

        private void RedrawSelectedResource() => 
            _enterResourceSelectionText.SetText($"{_selectedAmount}/{_maxAmount}");
    }
}