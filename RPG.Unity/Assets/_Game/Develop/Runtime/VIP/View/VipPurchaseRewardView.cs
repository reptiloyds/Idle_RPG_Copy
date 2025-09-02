using PleasantlyGames.RPG.Runtime.VIP.Model;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.VIP.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class VipPurchaseRewardView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _value;
        
        public void Setup(VipPurchaseReward model)
        {
            _image.sprite = model.Sprite;
            _background.color = model.Color;
            _value.SetText(model.Value);
        }
    }
}