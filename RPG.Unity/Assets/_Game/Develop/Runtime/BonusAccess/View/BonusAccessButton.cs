using System;
using PleasantlyGames.RPG.Runtime.BonusAccess.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.BonusAccess.View
{
    public partial class BonusAccessButton : BaseButton
    {
        [Serializable]
        private class Setup
        {
            public BonusButtonState State;
            public GameObject[] Objects;
            public Sprite ButtonSprite;
            public TextMeshProUGUI Label;
            public TextMeshProUGUI Additional;

            public void Enable(Image image)
            {
                foreach (var obj in Objects)
                    obj.SetActive(true);
                image.sprite = ButtonSprite;
            }

            public void Disable()
            {
                foreach (var obj in Objects)
                    obj.SetActive(false);
            }
        }
        
        [SerializeField] private BonusAccessType _accessType;
        [SerializeField] private UITimer _cooldownTimer;
        [SerializeField] private Image _buttonImage;
        
        [SerializeField] private Setup[] _setups;
        
        [ShowInInspector, HideInEditorMode, ReadOnly]
        private BonusButtonState _state;
        private Setup _stateSetup;

        private Action _externalMainAction;
        private ReactiveProperty<string> _labelInfo = new();
        private ReactiveProperty<string> _additionalInfo = new();
        private Func<bool> _executeCondition;
        private ReadOnlyReactiveProperty<float> _cooldown;
        private ReadOnlyReactiveProperty<bool> _isCooldownActive;
        private ReadOnlyReactiveProperty<int> _freeAccess;
        private ReadOnlyReactiveProperty<int> _usages;
        private CompositeDisposable _compositeDisposable;
        
        public bool IsFreeAccess => (_state & BonusButtonState.FreeAccess) != 0;
        public event Action OnExecuted;

        public BonusAccessButton SetLabelText(string labelText)
        {
            _labelInfo.Value = labelText;
            return this;
        }

        public BonusAccessButton SetAdditionalText(string additionalText)
        {
            _additionalInfo.Value = additionalText;
            return this;
        }

        public BonusAccessButton SetExecuteCondition(Func<bool> condition)
        {
            _executeCondition = condition;
            return this;
        }

        public BonusAccessButton SetCooldown(ReadOnlyReactiveProperty<float> cooldown)
        {
            _cooldown = cooldown;
            return this;
        }

        public BonusAccessButton SetFreeAccess(ReadOnlyReactiveProperty<int> freeAccess)
        {
            _freeAccess = freeAccess;
            return this;
        }

        public BonusAccessButton SetUsages(ReadOnlyReactiveProperty<int> usages)
        {
            _usages = usages;
            return this;
        }

        public BonusAccessButton SetMainAction(Action action)
        {
            _externalMainAction = action;
            return this;
        }

        public void Build()
        {
            _compositeDisposable = new CompositeDisposable();
            if (_labelInfo == null) 
                _labelInfo = new ReactiveProperty<string>("");
            if(_additionalInfo == null)
                _additionalInfo = new ReactiveProperty<string>("");
            if (_freeAccess == null)
                _freeAccess = new ReactiveProperty<int>(0);
            if (_usages == null)
                _usages = new ReactiveProperty<int>(-1);
            if (_cooldown == null) 
                _cooldown = new ReactiveProperty<float>(0);
            _isCooldownActive = _cooldown.Select(value => value > 0).ToReadOnlyReactiveProperty();

            switch (_accessType)
            {
                case BonusAccessType.Ad:
                    InitializeAdAccess();
                    break;
                case BonusAccessType.Vip:
                    InitializeVipAccess();
                    break;
            }
            
            UpdateState();
            
            _isCooldownActive.Subscribe((isActive) =>
                {
                    if (isActive)
                        _cooldownTimer.Listen(_cooldown);
                    else
                        _cooldownTimer.Stop();
                    UpdateState();
                })
                .AddTo(_compositeDisposable);
            
            _labelInfo
                .Subscribe(_ => UpdateLabel())
                .AddTo(_compositeDisposable);

            _additionalInfo
                .Subscribe(value => UpdateAdditionalInfo())
                .AddTo(_compositeDisposable);
        }

        public void Clear()
        {
            _compositeDisposable?.Dispose();
            _compositeDisposable = null;
            _labelInfo.Value = string.Empty;
            _additionalInfo.Value = string.Empty;
            _freeAccess = null;
            _usages = null;
            _cooldown = null;
            _isCooldownActive = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            switch (_accessType)
            {
                case BonusAccessType.Ad:
                    DisposeAdAccess();
                    break;
                case BonusAccessType.Vip:
                    DisposeVipAccess();
                    break;
            }
            _compositeDisposable?.Dispose();
        }

        [Button]
        public void UpdateState()
        {
            if(TryEnterInactiveState())
                return;
            if(TryEnterCooldownState())
                return;
            if(TryEnterFreeAccessState())
                return;

            TryEnterMainState();
        }

        private bool TryEnterInactiveState()
        {
            if (_usages.CurrentValue != 0) return false;
            EnterState(BonusButtonState.Inactive);
            return true;
        }

        private bool TryEnterCooldownState()
        {
            if (!_isCooldownActive.CurrentValue) return false;
            EnterState(BonusButtonState.Cooldown);
            return true;
        }

        private bool TryEnterFreeAccessState()
        {
            if (_freeAccess.CurrentValue == 0) return false;
            EnterState(BonusButtonState.FreeAccess);
            return true;
        }

        private void EnterState(BonusButtonState state)
        {
            _state = state;

            Setup currentSetup = null;
            foreach (var visualSetup in _setups)
            {
                if (visualSetup.State != state)
                    visualSetup.Disable();
                else
                    currentSetup = visualSetup;
            }
            
            EnableStateSetup(currentSetup);

            switch (_state)
            {
                case BonusButtonState.Inactive:
                    SetInteractable(false);
                    break;
                case BonusButtonState.FreeAccess:
                    SetInteractable(CanExecute());
                    break;
                case BonusButtonState.Cooldown:
                    SetInteractable(false);
                    break;
            }
        }

        private void EnableStateSetup(Setup setup)
        {
            if(setup == null) return;
            _stateSetup = setup;
            _stateSetup.Enable(_buttonImage);
            UpdateLabel();
            UpdateAdditionalInfo();
        }

        private void UpdateLabel()
        {
            if(_stateSetup.Label == null) return; 
            _stateSetup.Label.SetText(_labelInfo.CurrentValue);
        }

        private void UpdateAdditionalInfo()
        {
            if(_stateSetup.Additional == null) return;
            _stateSetup.Additional.SetText(_additionalInfo.CurrentValue);
        }

        private void TryEnterMainState()
        {
            switch (_accessType)
            {
                case BonusAccessType.Ad:
                    TryEnterAdState();
                    break;
                case BonusAccessType.Vip:
                    TryEnterVipState();
                    break;
            }
        }

        protected override void Click()
        {
            if (!CanExecute()) return;
            base.Click();

            if (BonusButtonState.Main.HasFlag(_state) && _externalMainAction == null)
            {
                switch (_state)
                {
                    case BonusButtonState.Ad:
                        ClickOnAd();
                        break;
                    case BonusButtonState.Vip:
                        ClickOnVip();
                        break;
                    default:
                        Execute();
                        break;
                }
            }
            else
                Execute();
        }

        [Button]
        private bool CanExecute()
        {
            if(_executeCondition == null) return true;
            return _executeCondition.Invoke();
        }

        private void Execute()
        {
            _externalMainAction?.Invoke();
            OnExecuted?.Invoke();
            UpdateState();
        }
    }
}