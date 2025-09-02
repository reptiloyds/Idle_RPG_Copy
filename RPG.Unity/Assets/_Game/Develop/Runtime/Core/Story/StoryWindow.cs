using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Story
{
    public class StoryWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private CanvasGroup _fadeAnimationTarget;
        
        [SerializeField] private BaseButton _button;
        [SerializeField] private Image _nextView;
        [SerializeField] private Image _currentView;
        [SerializeField] private List<Sprite> _storyList;
        [SerializeField] private float _fadeDuration = 0.5f;

        private int _spriteId = 0;
        private Action _callback;

        private Tween _tween;

        protected override void Awake()
        {
            base.Awake();
            _button.OnClick += OnClick;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _button.OnClick -= OnClick;
        }

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseFadeTween(_fadeAnimationTarget, UseUnscaledTime, callback, duration: _fadeDuration);

        private void OnClick()
        {
            _tween.Complete();
            _tween.Stop();
            
            _spriteId++;
            if (_spriteId >= _storyList.Count)
            {
                Complete();
                return;
            }

            _nextView.sprite = _storyList[_spriteId];
            _nextView.enabled = true;
            _tween = Tween.Alpha(_currentView, 0, _fadeDuration, useUnscaledTime: true)
                .OnComplete(() =>
                {
                    _currentView.sprite = _nextView.sprite;
                    _currentView.color = Color.white;
                    _nextView.enabled = false;
                });
        }

        public void Play(Action callback)
        {
            _spriteId = 0;
            _callback = callback;
            _currentView.sprite = _storyList[_spriteId];
            _nextView.enabled = false;
        }

        private void Complete()
        {
            Close();
            _callback?.Invoke();
        }
    }
}