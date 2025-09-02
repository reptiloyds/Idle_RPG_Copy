using System;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class HintPointerView : MonoBehaviour
    {
        [SerializeField] private PointerView _pointerView;
        [SerializeField] private float _hintDelay = 120;
        [SerializeField] private float _hintDuration = 60;

        private IDisposable _hintTimer;
        private IDisposable _durationTimer;
        
        private float _timer;
        private bool _isReady;
        
        private void Awake() => 
            DisablePointer();
        
        public void Ready()
        {
            if(_isReady) return;
            _isReady = true;

            StartDelayTimer();
        }

        public void NotReady()
        {
            _isReady = false;
            _hintTimer?.Dispose();
            _durationTimer?.Dispose();
            DisablePointer();
        }

        private void StartDelayTimer()
        {
            _timer = _hintDelay;
            _hintTimer?.Dispose();
            _hintTimer = Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    _timer -= Time.unscaledDeltaTime;
                    if (_timer > 0) return;
                    _hintTimer?.Dispose();
                    EnablePointer();
                });
        }

        private void EnablePointer()
        {
            _pointerView.gameObject.SetActive(true);

            _timer = _hintDuration;
            _durationTimer?.Dispose();
            _durationTimer = Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    _timer -= Time.unscaledDeltaTime;
                    if (_timer > 0) return;
                    _durationTimer?.Dispose();
                    DisablePointer();
                });
        }

        private void DisablePointer()
        {
            _pointerView.gameObject.SetActive(false);

            if (_isReady) 
                StartDelayTimer();
        }
    }
}