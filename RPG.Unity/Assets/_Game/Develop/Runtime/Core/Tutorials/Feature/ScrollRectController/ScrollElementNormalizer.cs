using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.ScrollRectController
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ScrollElementNormalizer : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _scrollElement;

        private ScrollRect _scrollRect;
        private const float _padding = 1f;
        
        private void Reset() => 
            _scrollElement = GetComponent<RectTransform>();

        public void NormalizeScroll()
        {
            RefreshScrollRect();
            if(_scrollRect == null) return;
            if(!IsElementVisible())
                ScrollToTarget();
        }

        public void DisableScroll()
        {
            RefreshScrollRect();
            if(_scrollRect == null) return;
            _scrollRect.enabled = false;
        }

        public void EnableScroll()
        {
            RefreshScrollRect();
            if(_scrollRect == null) return;
            _scrollRect.enabled = true;
        }

        private void ScrollToTarget()
        {
            RectTransform content = _scrollRect.content;
            float viewportHeight = _scrollRect.viewport.rect.height;
            float contentHeight = content.rect.height;

            // Локальная позиция элемента в контенте
            Vector3 localPos = content.InverseTransformPoint(_scrollElement.position);

            // Центр элемента относительно верха контента
            float elementCenterY = -localPos.y + _scrollElement.rect.height / 2f;

            // Желаемая позиция (центр элемента в центр вьюпорта)
            float targetY = elementCenterY - viewportHeight / 2f;

            // Переводим в normalized (1 = верх, 0 = низ)
            float normalized = 1f - Mathf.Clamp01(targetY / (contentHeight - viewportHeight));

            _scrollRect.verticalNormalizedPosition = normalized;
        }

        private void RefreshScrollRect()
        {
            if (_scrollRect == null) 
                _scrollRect = _scrollElement.GetComponentInParent<ScrollRect>();
        }
        
        [Button]
        private bool IsElementVisible()
        {
            RefreshScrollRect();
            if(_scrollRect == null) return false;
            RectTransform viewport = _scrollRect.viewport;
            Rect viewportRect = GetWorldRect(viewport);
            Rect targetRect = GetWorldRect(_scrollElement);
            
            viewportRect.xMin -= _padding;
            viewportRect.yMin -= _padding;
            viewportRect.xMax += _padding;
            viewportRect.yMax += _padding;
            
            var result = viewportRect.Contains(targetRect.min) && viewportRect.Contains(targetRect.max);
            return result;
        }
        
        private Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return new Rect(corners[0], corners[2] - corners[0]);
        }
    }
}