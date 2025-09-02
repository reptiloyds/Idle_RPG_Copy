using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components
{
    [DisallowMultipleComponent, HideMonoScript]
    public class DungeonRewardPresenter : MonoBehaviour
    {
        [SerializeField] private DungeonRewardCard _rewardCard;
        [SerializeField] private bool _formatRewardAmount = true;
        public Vector3 RewardPoint => _rewardCard.transform.position;

        public void SpawnReward(Sprite sprite, BigDouble.Runtime.BigDouble amount, Color color)
        {
            var amountString = _formatRewardAmount ? StringExtension.Instance.CutBigDouble(amount) : StringExtension.Instance.RoundBigDouble(amount); 
            _rewardCard.Setup(sprite, amountString, color);
            _rewardCard.Play();
        }
    }
}