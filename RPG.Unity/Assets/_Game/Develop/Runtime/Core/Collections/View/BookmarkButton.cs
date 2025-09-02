using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Collections.View
{
    public class BookmarkButton : BaseButton
    {
        [SerializeField] private Image _background;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField, MinValue(0)] private float _moveTime;
        [SerializeField] private Ease _moveEase;
        [SerializeField] private RectTransform _defaultPoint;
        [SerializeField] private RectTransform _selectPoint;
        [SerializeField] private RectTransform _visual;

        private Tween _tween;
        private bool _isSelected;

        public event Action<BookmarkButton> OnBookmarkClicked; 
            
        public void Select()
        {
            if(_isSelected) return;
            _isSelected = true;
            
            _background.color = _selectedColor;
            _tween.Stop();
            _visual.localPosition = _defaultPoint.localPosition;
            _tween = Tween.LocalPosition(_visual, _selectPoint.localPosition, _moveTime, _moveEase);
        }

        public void Deselect()
        {
            if(!_isSelected) return;
            _isSelected = false;
            
            _background.color = _defaultColor;
            _tween.Stop();
            _visual.localPosition = _selectPoint.localPosition;
            _tween = Tween.LocalPosition(_visual, _defaultPoint.localPosition, _moveTime, _moveEase);
        }

        protected override void Click()
        {
            base.Click();
            OnBookmarkClicked?.Invoke(this);
        }
    }
}