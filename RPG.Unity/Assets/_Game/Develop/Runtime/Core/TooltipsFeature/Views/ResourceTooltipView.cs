using PleasantlyGames.RPG.Runtime.Core.UI.ScreenClamper;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Views
{
    public class ResourceTooltipView : MonoBehaviour
    {
        [SerializeField, Required] private TMP_Text _name;
        [SerializeField, Required] private TMP_Text _description;
        [SerializeField, Required] private Image _icon;
        [SerializeField, Required] private RectTransform _rect;
        [SerializeField, Required] private ScreenFitter _screenFitter;

        public void SetupPosition(RectTransform target)
        {
            _rect.transform.position = target.transform.position;
            _screenFitter.Fit();
        }

        public void Setup(string resourceName, string description, Sprite icon)
        {
            _name.text = resourceName;
            _description.text = description;
            _icon.sprite = icon;
        }
    }
}