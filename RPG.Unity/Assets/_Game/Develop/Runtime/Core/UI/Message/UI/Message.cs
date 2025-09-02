using System;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Message.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    internal class Message : MonoBehaviour
    {
        [SerializeField, Required] private CanvasGroup _canvasGroup;
        [SerializeField, Required] private Transform _visual;
        [SerializeField, Required] private TextMeshProUGUI _infoText;
        [SerializeField] private TweenSettings<float> _showSettings;
        [SerializeField] private TweenSettings<float> _hideSettings;
        [SerializeField] private float _showDuration;

        private bool _isActive;
        
        public event Action<Message> OnComplete;
        public string Content => _infoText.text;

        public void Show(string text)
        {
            _infoText.SetText(text);

            var sequence = Sequence.Create();
            sequence.Chain(PrimeTween.Tween.Alpha(_canvasGroup, _showSettings));
            sequence.ChainDelay(_showDuration);
            sequence.Chain(PrimeTween.Tween.Alpha(_canvasGroup, _hideSettings));
            sequence.OnComplete(() => OnComplete?.Invoke(this));
        }
    }
}