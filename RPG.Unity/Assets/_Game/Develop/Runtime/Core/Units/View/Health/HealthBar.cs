using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Health
{
    [DisallowMultipleComponent, HideMonoScript]
    public class HealthBar : MonoBehaviour
    {
        [SerializeField, Required] private Slider _fill;
        [SerializeField, Required] private Slider _fakeFill;
        [SerializeField, Required] private RectTransform _rectTransform;
        [SerializeField, Required] private TextMeshProUGUI _valueView;
        [SerializeField, MinValue(0)] private float _fillTime = 0.5f;
        [SerializeField, Range(0, 1)] private float _minimalValue = 0.05f;

        private float _lastValue;

        private Tween _fakeFillTween;
        
        private Vector2 _originalSize;
        private UnitHealth _health;
        private Transform _originalParent;
        private UnityEngine.Camera _mainCamera;
        private bool _hasTarget;
        private Transform _target;
        private Vector3 _targetOffset;
        
        public void SetHealth(UnitHealth health)
        {
            _health = health;
            _health.OnHideFlagChanged += Redraw;
            _health.OnChangeMaxValue += OnChangeMaxValue;
            _health.OnDamage += OnDamage;
            _health.OnHeal += OnHeal;

            _fakeFill.value = 1;
            Redraw();
        }

        public void Initialize(UnityEngine.Camera mainCamera)
        {
            _mainCamera = mainCamera;
            DisableValueView();
            Redraw();
        }

        public void Dispose()
        {
            _hasTarget = false;
            _target = null;
            
            RemoveHealth();
            RevertSize();
        }

        public void ChangeSize(Vector2 size)
        {
            if (_originalSize == Vector2.zero) 
                _originalSize = _rectTransform.sizeDelta;
            
            _rectTransform.sizeDelta = size;
        }

        public void SetTarget(Transform target, Vector3 offset)
        {
            _hasTarget = true;
            _target = target;
            _targetOffset = offset;
        }

        private void Update() => 
            UpdatePosition();

        private void UpdatePosition()
        {
            if (_hasTarget) 
                SetPosition(_target.position + _targetOffset);
        }

        private void RevertSize()
        {
            if (_originalSize == Vector2.zero) return;
            _rectTransform.sizeDelta = _originalSize;
        }

        public void EnableValueView()
        {
            _valueView.gameObject.SetActive(true);
            RedrawValueView();
        }

        private void DisableValueView() => 
            _valueView.gameObject.SetActive(false);

        public void SetPosition(Vector3 position)
        {
            Vector2 uiPosition = _mainCamera.WorldToScreenPoint(position);
            transform.SetPositionAndRotation(uiPosition, Quaternion.identity);
        }

        private void OnHeal(BigDouble.Runtime.BigDouble value) => Redraw();

        private void OnDamage(BigDouble.Runtime.BigDouble value) => Redraw();

        private void OnChangeMaxValue() => Redraw();

        private void Redraw()
        {
            var newValue = _health.Value / _health.MaxValue;
            var newFloatValue = (float)newValue.ToDouble();
            if (_health.HideWhenFull)
            {
                if (newFloatValue >= 0.99)
                {
                    gameObject.SetActive(false);
                    return;
                }
                gameObject.SetActive(true);
                UpdatePosition();
            }
            
            if (_valueView != null && _valueView.gameObject.activeSelf)
                RedrawValueView();
            
            newFloatValue = Mathf.Max(newFloatValue, _minimalValue);
            _fill.value = newFloatValue;
            
            if (newValue >= _lastValue)
            {
                _fakeFill.value = newFloatValue;
            }
            else
            {
                _fakeFillTween.Stop();
                _fakeFillTween = Tween.UISliderValue(_fakeFill, _fakeFill.value, newFloatValue, _fillTime);
            }

            _lastValue = newFloatValue;
        }

        private void RedrawValueView() => 
            _valueView.SetText(StringExtension.Instance.CutBigDouble(_health.Value, true));

        private void RemoveHealth()
        {
            _health.OnHideFlagChanged -= Redraw;
            _health.OnChangeMaxValue -= OnChangeMaxValue;
            _health.OnDamage -= OnDamage;
            _health.OnHeal -= OnHeal;
            _health = null; 
        }
    }
}