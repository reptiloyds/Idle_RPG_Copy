using System;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Type;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base
{
    [HideMonoScript]
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region Visual
        
        [SerializeField, FoldoutGroup("Visual")] private ButtonVisual[] _buttonVisuals;
        [SerializeField, FoldoutGroup("Visual")] private UISoundType _sound = UISoundType.UI_Click;
        [SerializeField, FoldoutGroup("Visual")] private bool _handleFailClick;
        [SerializeField, FoldoutGroup("Visual"), HideIf("@this._handleFailClick == false")] private UISoundType _failSound = UISoundType.UI_Click;
        [SerializeField, FoldoutGroup("Visual")] private bool _punchScale;
        [SerializeField, FoldoutGroup("Visual"), HideIf("@this._punchScale == false")] private Transform _punchTarget;
        [SerializeField, FoldoutGroup("Visual"), HideIf("@this._punchScale == false")] private ShakeSettings _clickSettings = new ShakeSettings()
        {
            strength = -0.1f * Vector2.one,
            duration = 0.2f,
            frequency = 2,
            useUnscaledTime = true,
        };

        #endregion
        
        [SerializeField] private string _id;
        [SerializeField, Required] private UnityEngine.UI.Button _button;
        [SerializeField] private bool _isAlwaysAllowed;
        [SerializeField] private bool _isClamped; 
        [SerializeField, HideIf("@this._isClamped == false")] private float _startClampDelay = 0.15f;
        [SerializeField, HideIf("@this._isClamped == false")] private float _clampStep = 0.01f;
        [SerializeField, HideIf("@this._isClamped == false")] private float _minClampDelay = 0.05f;
        
        [Inject] private IAudioService _audioService;
        [Inject] private IButtonService _service;

        private AudioEmitter _clampAudioEmitter;
        private bool _clampIsEnabled;
        private float _clampDelay;
        private float _nextClampTime;

        [field: ReadOnly]
        public bool IsInteractable { get; private set; } = true;
        public bool IsPressed { get; private set; }
        public int VisualAccentCounter { get; private set; }

        public string Id => _id;
        public event Action OnVisualAccentChange;
        public event Action OnClick;
        public event Action OnFailClick;
        public event Action OnDown;
        public event Action OnUp;

        protected virtual void Reset() => 
            _button = GetComponent<UnityEngine.UI.Button>();

        public void ChangeButtonId(string id)
        {
            if(string.Equals(id, _id)) return;
            if(!string.IsNullOrEmpty(_id)) 
                _service.UnregisterButton(this);
            _id = id;
            if(!string.IsNullOrEmpty(_id))
                _service.RegisterButton(this);
        }

        protected virtual void Awake()
        {
            _service.RegisterButton(this);
            _button.onClick.AddListener(OnButtonClick);
        }

        protected virtual void OnDestroy()
        {
            _service.UnregisterButton(this);
            _button.onClick.RemoveListener(OnButtonClick);
        }

        protected void SetClickSound(UISoundType soundType) => _sound = soundType;

        protected UISoundType GetSoundType() => _sound;

        private void OnDisable() => 
            _clampIsEnabled = false;

        public virtual void SetInteractable(bool value)
        {
            IsInteractable = value;

            foreach (var buttonVisual in _buttonVisuals)
                buttonVisual.SetInteractable(IsInteractable);
        }

        public void AddVisualAccent()
        {
            VisualAccentCounter++;
            OnVisualAccentChange?.Invoke();
        }

        public void RemoveVisualAccent()
        {
            VisualAccentCounter--;
            OnVisualAccentChange?.Invoke();
        } 

        private void OnButtonClick()
        {
            if(_isClamped && IsPressed) return;
            TryClick();
        }

        private void TryClick()
        {
            if (_service.IsButtonInputBlocked && !_service.IsAllowedButton(Id) && !_isAlwaysAllowed) return;
            
            if (!IsInteractable)
            {
                if (_handleFailClick) FailClick();
                return;
            }

            Click();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if(IsPressed) return;
            
            IsPressed = true;

            if (_isClamped)
            {
                _clampIsEnabled = true;
                _clampDelay = _startClampDelay;
                _nextClampTime = Time.unscaledTime + _startClampDelay;
                _clampAudioEmitter = _audioService.CreateLocalSound(_sound).DontRelease().Build();
            }
            
            if(!IsInteractable) return;
            OnDown?.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if(!IsPressed) return;
            
            IsPressed = false;

            if (_isClamped)
            {
                _clampIsEnabled = false;
                _clampAudioEmitter.ReleaseOnEnd();
                _clampAudioEmitter = null;
            } 
            
            if(!IsInteractable) return;
            OnUp?.Invoke();
        }

        private void Update()
        {
            if(!_clampIsEnabled) return;
            if (Time.unscaledTime >= _nextClampTime)
            {
                if (_clampDelay >= _minClampDelay)
                {
                    _clampDelay -= _clampStep;
                    _clampDelay = Mathf.Max(_clampDelay, _minClampDelay);   
                }

                _nextClampTime = Time.unscaledTime + _clampDelay;
                TryClick();
            }
        }

        protected virtual void Click()
        {
            if (_clampAudioEmitter != null)
                _clampAudioEmitter.Play();
            else
                _audioService.CreateLocalSound(_sound).Play();
            OnClick?.Invoke();
            
            PunchScale();
            
            _service?.TriggerButtonClick(this);
        }

        protected virtual void FailClick()
        {
            _audioService.CreateLocalSound(_failSound).Play();
            PunchScale();
            
            OnFailClick?.Invoke();
        }
        
        private void PunchScale()
        {
            if(!_punchScale) return;
            var target = _punchTarget ? _punchTarget : transform;
            PrimeTween.Tween.PunchScale(target, _clickSettings);
        }
    }
}