using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.UI.InfinityScrolling
{
    [DisallowMultipleComponent, HideMonoScript]
    public class InfinityScroller<T> : MonoBehaviour 
    {
        [Header("UI References")]
        [SerializeField] private ExtendedScrollRect _scrollRect;
        [SerializeField] private RectTransform _content;
        [SerializeField] private InfinityScrollElement<T> _messagePrefab;
        [SerializeField] private InfinityScrollElement<T> _calculationElement;

        [Header("Settings")]
        [SerializeField] private int _poolSize = 20;
        [SerializeField] private float _spacing = 10f;
        [SerializeField] private float _padding = 20f;
        [SerializeField] private bool _autoScrollToBottom = true;

        [ShowInInspector, HideInEditorMode]
        public float VerticalNormalizedPosition => _scrollRect.verticalNormalizedPosition;

        private float _viewportHeight;
        private readonly List<ElementData> _allMessages = new();
        private readonly List<InfinityScrollElement<T>> _visibleItems = new();
        private int _renderedItems;
        private bool _isLastRendered;

        [Inject] private IObjectResolver _resolver;

        private class ElementData
        {
            public T Data;
            public float Height;
            public float YPosition;
        }

        public void Initialize()
        {
            _viewportHeight = _scrollRect.viewport.rect.height;
            _scrollRect.onValueChanged.AddListener(OnScroll);
            
            for (var i = 0; i < _poolSize; i++)
            {
                var item = _resolver.Instantiate(_messagePrefab, _content);
                item.gameObject.SetActive(false);
                _visibleItems.Add(item);
            }

            _messagePrefab.gameObject.SetActive(false);
        }

        public void Clear()
        {
            _allMessages.Clear();
            UpdateContentHeight();
            UpdateVisibleMessages();
        }
        
        [Button]
        public void AddMessage(T messageData)
        {
            _calculationElement.Setup(messageData);
            var height = _calculationElement.GetHeight();
            var y = _allMessages.Count > 0
                ? _allMessages[^1].YPosition - _allMessages[^1].Height - _spacing
                : -_padding;

            _allMessages.Add(new ElementData
            {
                Data = messageData,
                Height = height,
                YPosition = y
            });

            UpdateContentHeight();
            UpdateVisibleMessages();
            
            if (_autoScrollToBottom && !_scrollRect.IsDragging) 
                ScrollToBottom();
        }
        
        public void RemoveLast()
        {
            _allMessages.RemoveAt(_allMessages.Count - 1);
            UpdateContentHeight();
            UpdateVisibleMessages();
        }

        public void ScrollToBottom()
        {
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        public bool IsReadyToScrollDown()
        {
            return _allMessages.Count > _renderedItems && _scrollRect.verticalNormalizedPosition > 0.05f && !_isLastRendered;
        }

        private void UpdateContentHeight()
        {
            if (_allMessages.Count == 0) return;

            var totalHeight = -_allMessages[^1].YPosition + _allMessages[^1].Height + _padding;
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, totalHeight);
        }

        private void OnScroll(Vector2 delta) => 
            UpdateVisibleMessages();

        void UpdateVisibleMessages()
        {
            _renderedItems = 0;
            _isLastRendered = false;
            var scrollY = _content.anchoredPosition.y;
            var minY = scrollY - 100f;
            var maxY = scrollY + _viewportHeight + 100f;

            var startIndex = FindFirstVisibleIndex(minY);
            var endIndex = FindLastVisibleIndex(maxY);
            var visibleCount = endIndex - startIndex + 1;

            for (var i = 0; i < _visibleItems.Count; i++)
            {
                var item = _visibleItems[i];

                if (i < visibleCount)
                {
                    var dataIndex = startIndex + i;
                    if (dataIndex >= _allMessages.Count)
                    {
                        item.gameObject.SetActive(false);
                        continue;
                    }

                    var message = _allMessages[dataIndex];
                    if (dataIndex == _allMessages.Count - 1) 
                        _isLastRendered = true;

                    _renderedItems++;
                    item.gameObject.SetActive(true);
                    item.Setup(message.Data);
                    item.SetAnchoredPosition(new Vector2(0, message.YPosition));
                }
                else
                    item.gameObject.SetActive(false);
            }
        }

        private int FindFirstVisibleIndex(float minY)
        {
            for (var i = 0; i < _allMessages.Count; i++)
                if (-_allMessages[i].YPosition + _allMessages[i].Height > minY)
                    return i;

            return 0;
        }

        private int FindLastVisibleIndex(float maxY)
        {
            for (var i = 0; i < _allMessages.Count; i++)
                if (-_allMessages[i].YPosition > maxY)
                    return Mathf.Max(0, i - 1);

            return _allMessages.Count - 1;
        }
    }
}