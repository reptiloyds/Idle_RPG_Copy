using System;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BaseToggle : MonoBehaviour
    {
        [SerializeField] private BaseButton _baseButton;
        [SerializeField] private RectTransform _toggleObject;
        [SerializeField] private RectTransform _onPosition;
        [SerializeField] private RectTransform _offPosition;
        [SerializeField] private float _duration = 0.15f;
        [SerializeField] private Ease _ease = Ease.InOutSine;

        private bool _state;
        private PrimeTween.Tween _tween;
        
        public event Action OnStateChanged;

        private void Awake() => 
            _baseButton.OnClick += OnButtonClick;

        private void OnDestroy() => 
            _baseButton.OnClick -= OnButtonClick;

        private void OnButtonClick()
        {
            if(_state)
                Off();
            else
                On();
        }

        public void Setup(bool state)
        {
            _state = state;
            if (_state)
                _toggleObject.localPosition = _onPosition.localPosition;
            else
                _toggleObject.localPosition = _offPosition.localPosition;
        }
        
        private void On()
        {
            _state = true;
            _toggleObject.anchoredPosition = _offPosition.anchoredPosition;
            _tween.Stop();
            _tween = PrimeTween.Tween.LocalPosition(_toggleObject, _onPosition.localPosition, _duration, _ease);
            OnStateChanged?.Invoke();
        }

        private void Off()
        {
            _state = false;
            _toggleObject.anchoredPosition = _onPosition.anchoredPosition;
            _tween.Stop();
            _tween = PrimeTween.Tween.LocalPosition(_toggleObject, _offPosition.localPosition, _duration, _ease);
            OnStateChanged?.Invoke();
        }
    }
}