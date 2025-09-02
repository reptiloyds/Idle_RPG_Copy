using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View
{
    public class DungeonSweepRewardWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField] private Image _rewardImage;
        [SerializeField] private TextMeshProUGUI _rewardText;

        public Vector3 RewardPoint => _rewardImage.transform.position;

        public void Setup(Sprite sprite, BigDouble.Runtime.BigDouble amount)
        {
            _rewardImage.sprite = sprite;
            _rewardText.SetText(StringExtension.Instance.CutBigDouble(amount));
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);
    }
}