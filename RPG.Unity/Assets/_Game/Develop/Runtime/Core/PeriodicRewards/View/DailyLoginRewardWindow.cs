using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.ContentControl;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.View
{
    public class DailyLoginRewardWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField, MinValue(1)] private int _containerLimit = 4;
        [SerializeField] private RectTransform _upContainer;
        [SerializeField] private RectTransform _downContainer;
        [SerializeField] private PeriodicRewardView _viewPrefab;
        [SerializeField] private PeriodicRewardView _longViewPrefab;
        
        [Inject] private PeriodicRewardService _rewardService;
        [Inject] private ContentService _contentService;

        private int _rewardId;
        private readonly List<PeriodicRewardView> _periodicRewardViews = new();

        protected override void Awake()
        {
            base.Awake();
            CreateRewards();
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        public override void Close()
        {
            if (_rewardService.IsRewardReady(PeriodicRewardVariant.DailyLogin)
                && _contentService.GetById(ContentConst.DailyLoginReward).IsUnlocked)
            {
                var view = _periodicRewardViews[_rewardId];
                _rewardService.ApplyReward(PeriodicRewardVariant.DailyLogin, _rewardId, view.Rect.position);
                view.AnimateCollection(Close);   
            }
            else
                base.Close();
        }

        private void CreateRewards()
        {
            var model = _rewardService.GetModel(PeriodicRewardVariant.DailyLogin);
            _rewardId = model.RewardId;
            _name.SetText(model.Name);
            var rewards = model.GetRewards();
            var rewardId = model.RewardId;
            for (int i = 0; i < rewards.Count; i++)
            {
                var parent = i < _containerLimit ? _upContainer : _downContainer;
                var reward = rewards[i];
                var prefab = model.IsRewardBig(i) ? _longViewPrefab : _viewPrefab;
                var view = Instantiate(prefab, parent);
                view.Setup(reward.Image, reward.Color, model.GetRewardName(i), reward.Text, reward.TypeText);
                if (i < rewardId)
                    view.MarkCompleted();
                else if (i == rewardId)
                    view.MarkCurrent();
                _periodicRewardViews.Add(view);
            }
        }
    }
}