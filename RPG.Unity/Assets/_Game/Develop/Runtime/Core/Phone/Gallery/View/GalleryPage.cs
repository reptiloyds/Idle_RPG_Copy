using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Swiping;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.View
{
    public class GalleryPage : PhonePage
    {
        [SerializeField] private PhotoListView _listPrefab;
        [SerializeField] private RectTransform _listContainer;
        [SerializeField] private float _swipeAnimationDuration = 0.25f;
        [SerializeField] private Ease _swipeAnimationEase = Ease.OutSine;
        
        [SerializeField] private PhotoListButton _buttonPrefab;
        [SerializeField] private RectTransform _buttonsContainer;
        [SerializeField] [MinValue(2)] private int _photoInList = 8;

        [Inject] private GalleryService _galleryService;
        [Inject] private IObjectResolver _resolver;
        
        private PhotoListView _mainListView;
        private PhotoListView _nextListView;
        
        private readonly Dictionary<int, List<Photo>> _photoListMap = new();
        private readonly List<PhotoListButton> _buttons = new();
        
        private int _currentIndex;
        private Sequence _animationSequence;
        private bool _switchInProcess;

        public override void Initialize()
        {
            base.Initialize();

            _mainListView = _resolver.Instantiate(_listPrefab, _listContainer);
            _nextListView = _resolver.Instantiate(_listPrefab, _listContainer);
            _nextListView.gameObject.SetActive(false);
            FillLists();
        }

        private void OnDestroy() => 
            _mainListView.SwipeDetector.OnHorizontalSwipeDetected -= OnHorizontalSwipeDetected;

        private void OnHorizontalSwipeDetected(Vector2 swipe)
        {
            if (swipe.x > 0)
                SwitchTo(_currentIndex - 1);
            else
                SwitchTo(_currentIndex + 1);
        }

        public override void CloseSignal() => 
            Hide();

        public override void Show()
        {
            base.Show();
            _mainListView.Setup(_photoListMap[_currentIndex]);
        }

        public override void Hide()
        {
            base.Hide();
            _mainListView.Clear();
        }

        private void FillLists()
        {
            var photos = _galleryService.Photos;
            var listAmount = photos.Count / _photoInList;
            listAmount += photos.Count % _photoInList > 0 ? 1 : 0;
            for (var i = 0; i < listAmount; i++)
            {
                var list = new List<Photo>(_photoInList);
                for (var j = i * _photoInList; j < _photoInList * (i + 1); j++)
                {
                    if (j >= photos.Count) break;
                    list.Add(photos[j]);  
                } 
                _photoListMap.Add(i, list);
            }

            if (_photoListMap.Count == 0) return;

            for (var i = 0; i < _photoListMap.Count; i++)
            {
                var button = _resolver.Instantiate(_buttonPrefab, _buttonsContainer);
                button.OnListClicked += OnListClick;
                _buttons.Add(button);
            }

            _currentIndex = 0;
            _mainListView.Setup(_photoListMap[_currentIndex]);
            _nextListView.gameObject.SetActive(false);
            _mainListView.SwipeDetector.OnHorizontalSwipeDetected += OnHorizontalSwipeDetected;
            
            UpdateButtons();
        }

        private void OnListClick(PhotoListButton button)
        {
            if(_switchInProcess) return;
            var index = _buttons.IndexOf(button);
            if(index == _currentIndex) return;
            SwitchTo(index);
        }

        private void SwitchTo(int index)
        {
            if (!_photoListMap.TryGetValue(index, out var list)) return;

            _switchInProcess = true;
            var switchToRight = index > _currentIndex;
            _currentIndex = index;
            
            _nextListView.gameObject.SetActive(true);
            _nextListView.Setup(list);
            var nextListPositionX = switchToRight
                ? _mainListView.SelfRect.anchoredPosition.x + _mainListView.SelfRect.rect.width
                : _mainListView.SelfRect.anchoredPosition.x - _mainListView.SelfRect.rect.width;
            var mainListPositionX = -nextListPositionX;
            
            _nextListView.SelfRect.anchoredPosition = new Vector2(nextListPositionX, _mainListView.SelfRect.anchoredPosition.y);

            _animationSequence = Sequence.Create();
            _animationSequence.Group(Tween.UIAnchoredPosition(_mainListView.SelfRect,
                new Vector2(mainListPositionX, _mainListView.SelfRect.anchoredPosition.y),
                _swipeAnimationDuration, _swipeAnimationEase));
            _animationSequence.Group(Tween.UIAnchoredPosition(_nextListView.SelfRect,
                _mainListView.SelfRect.anchoredPosition,
                _swipeAnimationDuration, _swipeAnimationEase));
            _animationSequence.OnComplete(this, target => target.OnSwitchAnimationCompleted());
        }

        private void OnSwitchAnimationCompleted()
        {
            _switchInProcess = false;
            _mainListView.Clear();
            _mainListView.SwipeDetector.OnHorizontalSwipeDetected -= OnHorizontalSwipeDetected;
            (_mainListView, _nextListView) = (_nextListView, _mainListView);
            _mainListView.SwipeDetector.OnHorizontalSwipeDetected += OnHorizontalSwipeDetected;
            _nextListView.gameObject.SetActive(false);
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            for (var i = 0; i < _buttons.Count; i++)
            {
                if (i == _currentIndex) 
                    _buttons[i].Select();
                else
                    _buttons[i].Deselect();
            }
        }
    }
}