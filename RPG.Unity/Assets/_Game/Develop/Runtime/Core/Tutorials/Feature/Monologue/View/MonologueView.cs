using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Monologue.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class MonologueView : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _rectTransform;
        [SerializeField, Required] private TextMeshProUGUI _message;

        public RectTransform RectTransform => _rectTransform;

        public void SetText(string text) => 
            _message.SetText(text);
    }
}