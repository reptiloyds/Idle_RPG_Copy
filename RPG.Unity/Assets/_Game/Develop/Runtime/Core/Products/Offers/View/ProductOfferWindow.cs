using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Offers.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.Core.Products.View;
using PleasantlyGames.RPG.Runtime.Core.PurchasePresentation;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Offers.View
{
    public class ProductOfferWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private CanvasGroup _fadeAnimationTarget;
        [SerializeField] private TextMeshProUGUI _characterName;
        [SerializeField] private Image _characterRarity;
        [SerializeField] private Image _background;
        [SerializeField] private PurchaseProductButton _purchaseButton;
        [SerializeField] private PurchaseItemPresenter _itemPresenter;
        
        [Inject] private ProductService _service;
        
        private ProductOffer _productOffer;

        protected override void Awake()
        {
            base.Awake();
            _purchaseButton.OnClick += OnPurchaseClick;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _purchaseButton.OnClick -= OnPurchaseClick;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenFadeTween(_fadeAnimationTarget, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseFadeTween(_fadeAnimationTarget, UseUnscaledTime, callback);

        public void Setup(ProductOffer offer)
        {
            _purchaseButton.Setup(offer.Product);
            _productOffer = offer;
            _background.sprite = offer.Sprite;
            if (offer.Product.Rewards.Character != null)
            {
                _characterName.gameObject.SetActive(true);
                _characterRarity.gameObject.SetActive(true);
                _characterName.SetText(offer.Product.Rewards.Character.CharacterModel.FormattedName); 
                _characterRarity.sprite = offer.Product.Rewards.Character.CharacterModel.RarityImage;   
            }
            else
            {
                _characterName.gameObject.SetActive(false);
                _characterRarity.gameObject.SetActive(false);
            }
            
            _itemPresenter.Clear();
            foreach (var productItem in offer.Product.Rewards.List)
            {
                if(productItem.Type == ProductItemType.Character) continue;
                _itemPresenter.Append(productItem.Sprite, productItem.BackColor, productItem.Name, productItem.GetCertainProductType());  
            }
        }

        public override void Close()
        {
            base.Close();
            _purchaseButton.ClearProduct();
        }

        private void OnPurchaseClick() => 
            Purchase().Forget();

        private async UniTaskVoid Purchase()
        {
            var purchaseSuccess = await _service.Purchase(_productOffer.Product.Id);
            if (!purchaseSuccess) return;
            Close();
        }
    }
}