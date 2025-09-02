using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Swiping;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PhotoListView : MonoBehaviour
    {
        [SerializeField] private RectTransform _selfRect;
        [SerializeField] private PhotoView _viewPrefab;
        [SerializeField] private RectTransform _viewContainer;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private SwipeDetectorUI _swipeDetector;

        [Inject] private IObjectResolver _resolver;
        private readonly List<PhotoView> _views = new();

        public SwipeDetectorUI SwipeDetector => _swipeDetector;
        public RectTransform SelfRect => _selfRect;

        public void Setup(List<Photo> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var view = GetViewFor(i);
                view.gameObject.SetActive(true);
                view.Setup(list[i]);
            }

            for (var i = list.Count; i < _views.Count; i++)
            {
                _views[i].Clear();
                _views[i].gameObject.SetActive(false);
            }

            _scrollRect.verticalNormalizedPosition = 1;
        }

        private PhotoView GetViewFor(int index)
        {
            if (index < _views.Count) return _views[index];
            var view = _resolver.Instantiate(_viewPrefab, _viewContainer);
            _views.Add(view);
            return view;
        }

        public void Clear()
        {
            foreach (var view in _views)
                view.Clear();
        }
    }
}