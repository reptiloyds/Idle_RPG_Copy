using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Utilities.Loading
{
    [HideMonoScript, DisallowMultipleComponent]
    public class LoadingScreen : MonoBehaviour, IProgress<float>
    {
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private GameObject _fillBar;
        [SerializeField, Required] private Slider _slider;
        [SerializeField, Required] private TextMeshProUGUI _loadingInfo;
        [SerializeField, Required] private CanvasGroup _canvasGroup;
        [SerializeField, Min(0)] private float _barSpeed;
        [SerializeField, Min(0)] private float _fadeOutDuration;
        
        private float _targetProgress;
        private bool _isProgress;

        private Queue<ILoadUnit> _loadUnits;
        private ITranslator _translator;

        public bool IsLoading { get; private set; }
        public event Action OnComplete;

        public void Setup(ITranslator translator) => 
            _translator = translator;

        public async UniTask Load(Queue<ILoadUnit> loadUnits, CancellationToken token)
        {
            IsLoading = true;
            _loadUnits = loadUnits;
            await Load(token);
        }

        public async UniTask Hide()
        {
            Tween.Alpha(_canvasGroup, 0, _fadeOutDuration, useUnscaledTime: true);
            await UniTask.Delay((int)(_fadeOutDuration * 1000), DelayType.UnscaledDeltaTime);
        }

        private async UniTask Load(CancellationToken token)
        {
            while (true)
            {
                ResetFill();
                var operation = _loadUnits.Dequeue();
                _loadingInfo.SetText(_translator.Translate(operation.DescriptionToken));
                await operation.LoadAsync(token, this);
                token.ThrowIfCancellationRequested();

                if (_loadUnits.Count != 0) continue;
                _loadUnits = null;
                IsLoading = false;
                break;
            }
            OnComplete?.Invoke();
        }

        private void ResetFill()
        {
            _slider.value = 0;
            _targetProgress = 0;
        }

        void IProgress<float>.Report(float value) => 
            _targetProgress = value;

        private void Update()
        {
            if (_slider.value < _targetProgress)
                _slider.value += _barSpeed * Time.deltaTime;
        }
    }
}
