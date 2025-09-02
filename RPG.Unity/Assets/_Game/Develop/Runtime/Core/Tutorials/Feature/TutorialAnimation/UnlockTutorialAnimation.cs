using System;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.TutorialAnimation
{
    public class UnlockTutorialAnimation : TutorialAnimation
    {
        [SerializeField] private GameObject _blockObject;
        [SerializeField] private Image _background;
        [SerializeField, MinValue(0)] private float _value = 0.5f;
        [SerializeField, Range(0, 1)] private float _middlePoint = 0.7f;
        [SerializeField] private RectTransform _lock;
        [SerializeField] private Vector2 _lockTargetPositionUp = new Vector2(75, 175);
        [SerializeField] private Vector2 _lockTargetPositionDown = new Vector2(100, 130);
        [SerializeField] private Vector3 _lockTargetRotation = new Vector3(0, 0, -60);
        [SerializeField] private float _duration = 0.75f;
        [SerializeField] private ShakeSettings _shakeSettings = new ShakeSettings()
        {
            strength = new Vector3(10, 0, 0),
            duration = 0.75f,
            frequency = 4,
            useUnscaledTime = true
        };
        
        private Color _backColor;
        private Sequence _sequence;

        private void Awake()
        {
            _blockObject.SetActive(false);
            _backColor = _background.color;
        }

        public override void Play(Action callback, Action<TutorialAnimation> onComplete)
        {
            base.Play(callback, onComplete);

            var color = _backColor;
            color.a = _value;
            _background.color = color;
            _blockObject.SetActive(true);
            _lock.localPosition = Vector3.zero;
            _lock.localRotation = Quaternion.identity;

            _sequence = Sequence.Create(useUnscaledTime: true);
            _sequence.Chain(Tween.ShakeLocalPosition(_lock, _shakeSettings));
            _sequence.Chain(PlayLockAnimation());
            _sequence.Group(Tween.Alpha(_background, 0f, _duration, useUnscaledTime: true));
            _sequence.OnComplete(Complete);
        }

        private Sequence PlayLockAnimation()
        {
            var sequence = Sequence.Create(useUnscaledTime: true);
            var seq = Sequence.Create(useUnscaledTime: true);
            seq.Chain(Tween.UIAnchoredPosition(_lock, _lockTargetPositionUp, _duration * _middlePoint));
            seq.Chain(Tween.UIAnchoredPosition(_lock, _lockTargetPositionDown, _duration * (1 - _middlePoint)));
            sequence.Group(Tween.LocalRotation(_lock, _lockTargetRotation, _duration));
            sequence.Group(seq);
            return sequence;
        }

        protected override void Complete()
        {
            base.Complete();
            _blockObject.SetActive(false);
        }

        public override void Stop()
        {
            base.Stop();
            _blockObject.SetActive(false);
            _sequence.Stop();
        }
    }
}