using System;
using PleasantlyGames.RPG.Runtime.Core.Products.Offers.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Offers.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ProductOfferButton : MonoBehaviour
    {
        [SerializeField] private BaseButton _button;
        [SerializeField] private Image _image;
        
        public ProductOffer ProductOffer { get; private set; }
        public event Action<ProductOfferButton> OnClicked; 

        private void Awake() => 
            _button.OnClick += OnButtonClick;

        private void OnDestroy() => 
            _button.OnClick -= OnButtonClick;

        public void Setup(ProductOffer offer)
        {
            ProductOffer = offer;
            _image.sprite = offer.ButtonSprite;
        }

        private void OnButtonClick() => 
            OnClicked?.Invoke(this);
    }
}