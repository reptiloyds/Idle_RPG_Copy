using PleasantlyGames.RPG.Runtime.VIP.Model;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.VIP.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class VipBonusView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _definition;

        public void Setup(VipBonusData data)
        {
            _image.sprite = data.Sprite;
            _label.SetText(data.Label);
            _definition.SetText(data.Definition);
        }
    }
}