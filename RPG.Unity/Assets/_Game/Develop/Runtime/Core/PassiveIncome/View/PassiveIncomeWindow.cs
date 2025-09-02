using System;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.BonusAccess.View;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PassiveIncome.View
{
    public class PassiveIncomeWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private TextMeshProUGUI _incomeTimeText;
        [SerializeField] private Image _softImage;
        [SerializeField] private TextMeshProUGUI _softIncomeText;
        [SerializeField] private TextMeshProUGUI _softIncomeSpeedText;
        [SerializeField] private BaseButton _collectButton;
        [SerializeField] private GameObject _collectObject;
        [SerializeField] private TextTimer _timer;
        [SerializeField] private BonusAccessButton _bonusButton;

        [Inject] private PassiveIncomeModel _model;
        [Inject] private ResourceService _resourceService;
        [Inject] private IAdService _adService;

        private CompositeDisposable _compositeDisposable;
        
        protected override void Awake()
        {
            base.Awake();
            
            _collectButton.OnClick += OnCollectClick;

            _bonusButton
                .SetLabelText($"X{_model.BonusIncomeK}")
                .SetExecuteCondition(() => _model.SoftIncome.CurrentValue > 0)
                .SetCooldown(_model.Cooldown)
                .Build();
            
            _bonusButton.OnExecuted += Executed;

            _softImage.sprite = _resourceService.GetResource(ResourceType.Soft)?.Sprite;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _collectButton.OnClick -= OnCollectClick;
            _bonusButton.OnExecuted -= Executed;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        public override void Open()
        {
            base.Open();
            
            RedrawTime();
            RedrawSoftIncome();
            _softIncomeSpeedText.SetText(_model.GetFormatedOnlineIncomeSpeed());

            _compositeDisposable = new CompositeDisposable();
            _model.SoftIncome
                .Subscribe(_ => OnSoftChange())
                .AddTo(_compositeDisposable);
            _model.IsCooldown
                .Subscribe((_) => RedrawCooldown())
                .AddTo(_compositeDisposable);
            
            RedrawCooldown();
        }

        public override void Close()
        {
            base.Close();
            
            _compositeDisposable?.Dispose();
        }

        private void OnSoftChange()
        {
            RedrawTime();
            RedrawSoftIncome();
            RedrawCollectButton();
            _bonusButton.UpdateState();
        }

        private void RedrawTime() => 
            _incomeTimeText.SetText(_model.GetFormatedIncomeTime());

        private void RedrawSoftIncome() => 
            _softIncomeText.SetText(StringExtension.Instance.CutBigDouble(_model.SoftIncome.CurrentValue));

        private void OnCollectClick() => 
            _model.CollectIncome(_softIncomeText.transform.position);

        private void Executed() => 
            _model.CollectBonusIncome(_softIncomeText.transform.position);

        private void RedrawCooldown()
        {
            var isCooldownActive = _model.IsCooldown.CurrentValue;
            if (isCooldownActive)
                _timer.Listen(_model.Cooldown);
            else
                _timer.Stop();

            RedrawCollectButton();
            _timer.gameObject.SetActive(isCooldownActive);
            _collectObject.SetActive(!isCooldownActive);
        }

        private void RedrawCollectButton() => 
            _collectButton.SetInteractable(!_model.IsCooldown.CurrentValue && _model.SoftIncome.CurrentValue > 0);
    }
}