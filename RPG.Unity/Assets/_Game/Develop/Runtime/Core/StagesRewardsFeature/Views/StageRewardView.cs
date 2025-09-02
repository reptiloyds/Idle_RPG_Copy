using PleasantlyGames.RPG.Runtime.Core.UI.Tween;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Views
{
    public class StageRewardView : MonoBehaviour
    {
        [SerializeField, Required] private Transform _rewardContainer;
        [SerializeField, Required] private Image _rewardImage;
        [SerializeField, Required] private Image _completeImage;
        [SerializeField, Required] private UIShakeScale _scaleShakeAnimation;

        public float AnimationDuration => _scaleShakeAnimation.Duration;

        public void Setup(Sprite icon)
        {
            _rewardImage.sprite = icon;
        }

        public void EnableCompleteImage()
        {
            _rewardContainer.gameObject.Off();
            _completeImage.gameObject.On();
        }

        public void DisableCompleteImage()
        {
            _completeImage.gameObject.Off();
            _rewardContainer.gameObject.On();
        }

        public void PlayCompleteEffect()
        {
            EnableCompleteImage();
            _scaleShakeAnimation.Play();
        }
    }
}