using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View.Periodic
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PeriodicLabel : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private TextMeshProUGUI _text;

        public RectTransform Rect => _rect;

        public void SetText(string text) => 
            _text.SetText(text);
    }
}