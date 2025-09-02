using System;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    public class PurchaseProductButton : MonoBehaviour
    {
        [SerializeField] private GameObject _purchasedObject;
        [SerializeField] private BaseButton _button;
        [SerializeField] private Image _currencyImage;
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private UITimer _timer;
        
        private CompositeDisposable _disposable;
        private bool _isPriceOnCooldown;

        private Product _product;

        public BaseButton Button => _button;
        public event Action OnClick;

        private void Awake()
        {
            _button.OnClick += OnButtonClick;
            StopPriceCooldown();
        }

        protected void OnDestroy()
        {
            _button.OnClick -= OnButtonClick;
            ClearProduct();
        }

        private void OnButtonClick() => 
            OnClick?.Invoke();

        public void Setup(Product product)
        {
            ClearProduct();
            _product = product;
            
            _disposable = new CompositeDisposable();
            _product.Price.Value
                .Subscribe(value =>
                {
                    _currencyImage.gameObject.SetActive(!_product.Price.IsFree.CurrentValue);
                    _buttonText.SetText(value);
                })
                .AddTo(_disposable);
            product.Price.Cooldown
                .Pairwise()
                .Where((tuple) => tuple is { Previous: <= 0, Current: > 0 } or { Previous: > 0, Current: <= 0 })
                .Subscribe(_ => OnCooldownUpdate())
                .AddTo(_disposable);
        }

        public void ClearProduct()
        {
            if (_product == null) return;
            _disposable?.Dispose();
            _product = null;
            StopPriceCooldown();
        }

        public void Redraw()
        {
            _purchasedObject.SetActive(_product.Access.LimitIsOver);
            _button.gameObject.SetActive(!_product.Access.LimitIsOver);
        }

        private void OnCooldownUpdate()
        {
            if (_product.Price.Cooldown.CurrentValue > 0)
            {
                if(_isPriceOnCooldown) return;
                StartPriceCooldown();
            }
            else
            {
                if(!_isPriceOnCooldown) return;
                StopPriceCooldown();
            }
        }

        private void StartPriceCooldown()
        {
            _isPriceOnCooldown = true;
            _timer.gameObject.SetActive(true);
            _buttonText.gameObject.SetActive(false);
            _button.SetInteractable(false);
            _timer.Listen(_product.Price.Cooldown, false);
        }

        private void StopPriceCooldown()
        {
            _isPriceOnCooldown = false;
            _timer.gameObject.SetActive(false);
            _buttonText.gameObject.SetActive(true);
            _button.SetInteractable(true);
            _timer.Stop();
        }
    }
}