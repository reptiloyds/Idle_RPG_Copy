using System;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.View
{
    public class DailiesWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private BaseButton _receiveAllButton;
        [SerializeField] private RectTransform _dailiesContainer;
        [SerializeField] private DailyView _dailyViewPrefab;
        [SerializeField] private TextTimer _timer;

        private DailyView[] _views;

        [Inject] private DailyService _dailyService;
        [Inject] private IObjectResolver _objectResolver;

        protected override void Awake()
        {
            base.Awake();
            _receiveAllButton.OnClick += OnReceiveAllClick;
            
            CreateViews();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var view in _views)
            {
                if(view == null) continue;
                view.OnRewarded -= OnRewarded;
            }
            _receiveAllButton.OnClick -= OnReceiveAllClick;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        public override void Open()
        {
            base.Open();
            
            _dailyService.OnDailyCompleted += OnDailyCompleted;
            _dailyService.OnDailyCollected += OnDailyCollected;
            _dailyService.OnResetProgress += OnResetProgress;

            foreach (var view in _views!) 
                view.OnOpen();

            RedrawAll();
        }

        public override void Close()
        {
            base.Close();
            _dailyService.OnDailyCompleted -= OnDailyCompleted;
            _dailyService.OnDailyCollected -= OnDailyCollected;
            _dailyService.OnResetProgress -= OnResetProgress;
            
            if(_views != null)
                foreach (var view in _views) 
                    view.OnClose();
        }

        private void OnResetProgress() => 
            RedrawAll();

        private void RedrawAll()
        {
            RedrawReceiveAllButton();
            SortViews();
            _timer.Listen(_dailyService.DelayToReset);
        }

        private void OnDailyCollected(Daily daily)
        {
            RedrawReceiveAllButton();
            SortViews();
        }

        private void OnDailyCompleted(Daily daily)
        {
            RedrawReceiveAllButton();
            SortViews();
        }

        private void RedrawReceiveAllButton()
        {
            foreach (var view in _views)
            {
                if (!view.Button.IsFreeAccess || !view.Button.IsInteractable) continue;
                _receiveAllButton.SetInteractable(true);
                return;
            }
            _receiveAllButton.SetInteractable(false);
        }

        private int GetViewPriority(DailyView view)
        {
            var daily = view.Daily;

            if (!daily.IsBonus && daily.IsComplete && daily.HasReward) return 0;
            if (daily.IsBonus && daily.IsComplete && daily.HasReward) return 1;
            if (!daily.IsBonus && !daily.IsComplete) return 2;
            if (daily.IsBonus && !daily.IsComplete) return 3;
            if (!daily.IsBonus && !daily.HasReward) return 4;
            if (daily.IsBonus && !daily.HasReward) return 5;

            return 6;
        }

        public void SortViews()
        {
            var orderedViews = _views.OrderBy(GetViewPriority).ToList();
            for (var i = 0; i < orderedViews.Count; i++) 
                orderedViews[i].transform.SetSiblingIndex(i);
        }

        private void CreateViews()
        {
            _views = new DailyView[_dailyService.Dailies.Count];
            for (var i = 0; i < _dailyService.Dailies.Count; i++)
            {
                var view = _objectResolver.Instantiate(_dailyViewPrefab, _dailiesContainer);
                view.Setup(_dailyService.Dailies[i]);
                view.OnRewarded += OnRewarded;
                _views[i] = view;
            } 
        }

        private void OnReceiveAllClick()
        {
            _dailyService.OnDailyCollected -= OnDailyCollected;
            foreach (var view in _views)
            {
                if(!view.Button.IsFreeAccess || !view.Button.IsInteractable) continue;
                _dailyService.CollectReward(view.Daily, view.RewardPoint.position);
            }
            RedrawReceiveAllButton();
            SortViews();
            _dailyService.OnDailyCollected += OnDailyCollected;
        }

        private void OnRewarded(DailyView view) => 
            _dailyService.CollectReward(view.Daily, view.RewardPoint.position);

        public DailyView GetViewByModel(Daily daily)
        {
            foreach (var view in _views)
            {
                if (view.Daily != daily) continue;
                return view;
            }

            return null;
        }
    }
}