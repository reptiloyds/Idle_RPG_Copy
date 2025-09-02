using PleasantlyGames.RPG.Runtime.Core.Products.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.VIP.View
{
    public class VipWindow : BaseWindow
    {
        [SerializeField] private PurchaseProductButton _purchaseButton;
        [SerializeField] private GameObject _durationObject;
        [SerializeField] private UITimer _duration;
        [SerializeField] private VipPurchaseRewardsView _purchaseRewards;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _bigBonusDefinition;
        [SerializeField] private RectTransform _bonusContainer;
        [SerializeField] private VipBonusView _bonusPrefab;
        
        [Inject] private VipService _service;

        protected override void Awake()
        {
            base.Awake();
            _purchaseButton.OnClick += OnPurchaseClick;
            
            Initialize();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _purchaseButton.OnClick -= OnPurchaseClick;
        }

        private async void OnPurchaseClick()
        {
            _purchaseButton.Button.SetInteractable(false);
            await _service.TryActivate();
            _purchaseButton.Button.SetInteractable(true);
            _purchaseButton.Redraw();
        }

        private void Initialize()
        {
            _purchaseButton.Setup(_service.Product);
            _purchaseButton.Redraw();
            _service.IsActive.Subscribe((value) => RedrawState()).AddTo(this);
            _purchaseRewards.Setup(_service.PurchaseRewards);
            foreach (var bonusData in _service.BonusData)
            {
                var view = Instantiate(_bonusPrefab, _bonusContainer);
                view.Setup(bonusData);
            }
            _bigBonusDefinition.SetText(_service.GetRouletteDefinitions());
            _label.SetText(_service.GetLabel());
        }

        private void RedrawState()
        {
            if (_service.IsActive.CurrentValue)
            {
                _durationObject.SetActive(true);
                _duration.Listen(_service.Duration);
                _purchaseButton.gameObject.SetActive(false);
            }
            else
            {
                _durationObject.SetActive(false);
                _purchaseButton.gameObject.SetActive(true);
            }
            _purchaseButton.Redraw();
        }
    }
}