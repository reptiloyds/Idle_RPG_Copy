using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.InfinityScrolling
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class InfinityScrollElement<T> : MonoBehaviour
    {
        [SerializeField] protected RectTransform _selfRect;

        public void SetAnchoredPosition(Vector2 position) => 
            _selfRect.anchoredPosition = position;

        public float GetHeight() => 
            _selfRect.rect.height;

        public abstract void Setup(T data);
    }
}