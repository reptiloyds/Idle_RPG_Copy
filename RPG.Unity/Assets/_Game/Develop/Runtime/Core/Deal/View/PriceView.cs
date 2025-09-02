using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Deal.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PriceView : MonoBehaviour
    {
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private TextMeshProUGUI _text;

        public void SetImage(Sprite sprite)
        {
            _image.gameObject.SetActive(sprite != null);
            _image.sprite = sprite;
        }

        public void SetText(string text) => 
            _text.SetText(text);

        public void Enable()
        {
            _image.gameObject.SetActive(true);
            _text.gameObject.SetActive(true);
        }

        public void Disable()
        {
            _image.gameObject.SetActive(false);
            _text.gameObject.SetActive(false);
        }
    }
}