using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.SlotMachine
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        private bool _isDetected;

        public Sprite Sprite => _image.sprite;
        public string Text => _text.text;
        public RectTransform RectTransform => _rectTransform;
        public bool IsDetected => _isDetected;

        public void Setup(Sprite sprite, string text)
        {
            _isDetected = false;
            _image.sprite = sprite;
            _text.SetText(text);
        }

        public void Detect() => 
            _isDetected = true;
    }
}
