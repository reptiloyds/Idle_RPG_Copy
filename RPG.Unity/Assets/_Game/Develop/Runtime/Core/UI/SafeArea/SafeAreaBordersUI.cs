using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.SafeArea
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SafeAreaBordersUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;  // Контейнер, в котором будут созданы бордеры (обычно — Canvas или Panel)
        [SerializeField] private SafeAreaUI _safeAreaUI;     // Компонент, который применяет SafeArea
        [SerializeField] private Color _color = Color.black; // Цвет бордеров

        private readonly List<Image> _borders = new();

        private void Awake()
        {
            _safeAreaUI.OnApplied += OnSafeAreaApplied;
            RebuildBorders();
        }

        private void OnDestroy() => 
            _safeAreaUI.OnApplied -= OnSafeAreaApplied;

        private void OnSafeAreaApplied() => 
            RebuildBorders();

        private void RebuildBorders()
        {
            // Очистка старых бордеров
            foreach (var border in _borders)
            {
                if (border != null)
                    Destroy(border.gameObject);
            }
            _borders.Clear();

            var safe = _safeAreaUI.RectTransform;

            Vector2 anchorMin = safe.anchorMin;
            Vector2 anchorMax = safe.anchorMax;

            // Левый бордер (от 0 до anchorMin.x)
            if (anchorMin.x > 0)
                _borders.Add(CreateAnchoredBorder(
                    new Vector2(0, 0),
                    new Vector2(anchorMin.x, 1)));

            // Правый бордер (от anchorMax.x до 1)
            if (anchorMax.x < 1)
                _borders.Add(CreateAnchoredBorder(
                    new Vector2(anchorMax.x, 0),
                    new Vector2(1, 1)));

            // Нижний бордер (от 0 до anchorMin.y)
            if (anchorMin.y > 0)
                _borders.Add(CreateAnchoredBorder(
                    new Vector2(0, 0),
                    new Vector2(1, anchorMin.y)));

            // Верхний бордер (от anchorMax.y до 1)
            if (anchorMax.y < 1)
                _borders.Add(CreateAnchoredBorder(
                    new Vector2(0, anchorMax.y),
                    new Vector2(1, 1)));
        }

        private Image CreateAnchoredBorder(Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject obj = new GameObject("SafeBorder", typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(_container, false);

            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            Image image = obj.GetComponent<Image>();
            image.color = _color;

            return image;
        }

    }
}
