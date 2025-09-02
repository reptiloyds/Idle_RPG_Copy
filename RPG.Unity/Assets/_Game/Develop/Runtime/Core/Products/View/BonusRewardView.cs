using PleasantlyGames.RPG.Runtime.Core.Products.Rewards;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BonusRewardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _image;
        [SerializeField] private Image _background;

        public void Show(ProductReward reward)
        {
            gameObject.SetActive(true);
            _name.SetText($"+ {reward.Name}");
            _image.sprite = reward.Sprite;
            _background.color = reward.BackColor;
        }
        
        public void Hide() => 
            gameObject.SetActive(false);
    }
}