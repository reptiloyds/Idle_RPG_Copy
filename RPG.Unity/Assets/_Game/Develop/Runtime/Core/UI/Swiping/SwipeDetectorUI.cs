using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Swiping
{
    public class SwipeDetectorUI : MonoBehaviour
    {
        [SerializeField]
        private float _minSwipeDistance = 100f;

        private Vector2 _startTouchPos;
        private Vector2 _endTouchPos;

        private bool _touchStarted;

        public event Action<Vector2> OnSwipeDetected;
        public event Action<Vector2> OnVerticalSwipeDetected;
        public event Action<Vector2> OnHorizontalSwipeDetected;

        private void OnEnable()
        {
            _touchStarted = false;
        }

        private void Update()
        {
            if (Touchscreen.current == null || Touchscreen.current.touches.Count == 0)
                return;

            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                _startTouchPos = touch.position.ReadValue();
                _touchStarted = true;
            }

            if (touch.press.wasReleasedThisFrame && _touchStarted)
            {
                _endTouchPos = touch.position.ReadValue();
                _touchStarted = false;
                DetectSwipe();
            }
        }

        private void OnDisable()
        {
            _touchStarted = false;
        }

        private void DetectSwipe()
        {
            var swipeVector = _endTouchPos - _startTouchPos;

            if (swipeVector.magnitude < _minSwipeDistance)
                return;

            OnSwipeDetected?.Invoke(swipeVector);

            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
                OnHorizontalSwipeDetected?.Invoke(swipeVector);
            else
                OnVerticalSwipeDetected?.Invoke(swipeVector);
        }
    }
}