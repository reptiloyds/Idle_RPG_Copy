using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Accent.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class AccentView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        public RectTransform RectTransform => _rectTransform;
        
        //TODO MAKE A CHILD OF TARGET
        
        public void ApplySettings(RectTransform target, RectTransform parent, AccentSettings settings)
        {
            _rectTransform.sizeDelta = new Vector2(target.rect.width + settings.HorizontalOffset * 2, target.rect.height + settings.VerticalOffset * 2);
            _rectTransform.position = target.position;
            if (parent != null) 
                _rectTransform.SetParent(parent);
            transform.localScale = Vector3.one;
        }
    }
}