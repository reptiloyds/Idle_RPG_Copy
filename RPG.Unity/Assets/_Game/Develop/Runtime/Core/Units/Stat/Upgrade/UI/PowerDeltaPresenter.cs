using System.Collections.Generic;
using AssetKits.ParticleImage;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Model;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PowerDeltaPresenter : MonoBehaviour, IInitializable, ITickable
    {
        [SerializeField, MinValue(0)] private float _emergencyDuration = 0.15f;
        [SerializeField, MinValue(0)] private float _disappearDuration = 0.25f;
        [SerializeField, MinValue(0)] private float _duration = 0.35f;
        [SerializeField] private ShakeSettings _punchPowerSettings;
        [SerializeField] private List<Transform> _shakeTargets;
        [SerializeField, Required] private TextMeshProUGUI _powerText;
        [SerializeField, Required] private TextMeshProUGUI _deltaText; 
        [SerializeField, Required] private Image _deltaImage;
        [SerializeField] private Vector3 _positiveImageRotation;
        [SerializeField] private Vector3 _negativeImageRotation;
        [SerializeField] private Color _positiveColor;
        [SerializeField] private Color _negativeColor;
        [SerializeField, Required] private ParticleImage _particleImage;
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _redraw;
        private BigDouble.Runtime.BigDouble _firstPower;
        private BigDouble.Runtime.BigDouble _currentPower;

        private Sequence _sequence;

        [Inject] private PowerCalculator _powerCalculator;

        void IInitializable.Initialize()
        {
            Disable();
            _currentPower = _powerCalculator.Power.Value;
            _powerCalculator.OnPowerChanged += OnPowerChanged;   
        }

        private void OnDestroy()
        {
            if(_powerCalculator != null)
                _powerCalculator.OnPowerChanged -= OnPowerChanged;
        }

        private void OnPowerChanged(BigDouble.Runtime.BigDouble lastValue, BigDouble.Runtime.BigDouble newValue) => 
            RedrawPower(lastValue, newValue);

        private void RedrawPower(BigDouble.Runtime.BigDouble lastValue, BigDouble.Runtime.BigDouble newValue)
        {
            if (_firstPower == 0) _firstPower = lastValue;
            _currentPower = newValue;

            _redraw = !(BigDouble.Runtime.BigDouble.Abs(_firstPower - _currentPower) < 0.01);
        }

        private void Show()
        {
            Enable();
            _sequence.Stop();
            _sequence = Sequence.Create(useUnscaledTime: true);
            _sequence.Chain(Tween.Alpha(_canvasGroup, 1, _emergencyDuration, useUnscaledTime: true));
            _sequence.ChainDelay(_duration);
            _sequence.Chain(Tween.Alpha(_canvasGroup, 0, _disappearDuration, useUnscaledTime: true));
            _sequence.OnComplete(Disable);

            var delta = _currentPower - _firstPower;
            if (delta == 0)
                DisableDelta();
            else if (delta < 0)
                SetNegativeDelta();
            else
            {
                _particleImage.Play();
                foreach (var shakeTarget in _shakeTargets) 
                    Tween.PunchScale(shakeTarget, _punchPowerSettings);
                SetPositiveDelta();
            }
            _powerText.SetText(StringExtension.Instance.CutBigDouble(_currentPower));
            _deltaText.SetText(StringExtension.Instance.CutBigDouble(BigDouble.Runtime.BigDouble.Abs(delta)));
        }

        public void Tick()
        {
            if (!_redraw) 
                return;
            
            Show();
            _redraw = false;
            _firstPower = 0;
            _currentPower = 0;
        }

        private void Enable() => 
            gameObject.SetActive(true);

        private void Disable() => 
            gameObject.SetActive(false);

        private void SetPositiveDelta()
        {
            _deltaText.color = _positiveColor;
            _deltaImage.color = _positiveColor;
            _deltaImage.transform.rotation = Quaternion.Euler(_positiveImageRotation);
            _deltaImage.gameObject.SetActive(true);
            _deltaText.gameObject.SetActive(true);
        }

        private void SetNegativeDelta()
        {
            _deltaText.color = _negativeColor;
            _deltaImage.color = _negativeColor;
            _deltaImage.transform.rotation = Quaternion.Euler(_negativeImageRotation);
            _deltaImage.gameObject.SetActive(true);
            _deltaText.gameObject.SetActive(true);
        }

        private void DisableDelta()
        {
            _deltaImage.gameObject.SetActive(false);
            _deltaText.gameObject.SetActive(false);
        }
    }
}