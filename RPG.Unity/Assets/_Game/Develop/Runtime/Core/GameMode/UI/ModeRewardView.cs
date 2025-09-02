using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ModeRewardView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _background;
        [SerializeField] private Color _defaultBackColor = Color.white;
        [SerializeField] private TextMeshProUGUI _text;

        public void Setup(Sprite sprite, string text, Color backgroundColor = default)
        {
            _image.sprite = sprite;
            _text.SetText(text);
            _background.color = backgroundColor == default ? _defaultBackColor : backgroundColor;
        }
    }
}