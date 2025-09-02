using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components
{
    [DisallowMultipleComponent, HideMonoScript]
    public class DungeonRewardView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private bool _formatReward = true;

        public void Redraw(Sprite sprite, BigDouble.Runtime.BigDouble reward)
        {
            _image.sprite = sprite;
            _text.SetText(_formatReward ? StringExtension.Instance.CutBigDouble(reward) : StringExtension.Instance.RoundBigDouble(reward));
        }
    }
}