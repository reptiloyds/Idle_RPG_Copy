using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Products.Offers.Model;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Core.UI.Hub.Sides;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Offers.View.Mediator
{
    public class UIProductOfferMediator
    {
        [Inject] private ProductOfferService _productOfferService;
        [Inject] private SidesContainer _sidesContainer;
        [Inject] private UIFactory _uiFactory;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IWindowService _windowService;

        private ProductOfferButton _buttonPrefab;
        private readonly Dictionary<ProductOffer, ProductOfferButton> _buttonDictionary = new();

        [Preserve]
        public UIProductOfferMediator()
        {
            
        }
        
        public async UniTask InitializeAsync()
        {
            var offerButtonObject = await _uiFactory.LoadAsync(Asset.UI.ProductOfferButton, false);
            _buttonPrefab = offerButtonObject.GetComponent<ProductOfferButton>();
            
            _productOfferService.OnOfferReady += OnOfferReady;
            _productOfferService.OnOfferCompleted += OnOfferCompleted;
            foreach (var offer in _productOfferService.Offers)
            {
                if (offer.IsReady)
                    await ShowOfferAsync(offer);
            }
        }

        private async void OnOfferReady(ProductOffer offer) => 
            ShowOfferAsync(offer).Forget();

        private async UniTask ShowOfferAsync(ProductOffer offer)
        {
            var parent = _sidesContainer.GetSide(offer.UISideType);
            var button = _objectResolver.Instantiate(_buttonPrefab, parent);
            button.Setup(offer);
            button.OnClicked += OnButtonClicked;
            _buttonDictionary[offer] = button;
            await OpenPopup(offer);
        }

        private void OnOfferCompleted(ProductOffer offer)
        {
            var button = _buttonDictionary[offer];
            _buttonDictionary.Remove(offer);
            button.OnClicked -= OnButtonClicked;
            Object.Destroy(button.gameObject);
        }

        private async void OnButtonClicked(ProductOfferButton button) => 
            await _windowService.OpenAsync<ProductOfferWindow>(offerWindow => offerWindow.Setup(button.ProductOffer));

        private async UniTask OpenPopup(ProductOffer offer) => 
            await _windowService.PushPopupAsync<ProductOfferWindow>(offerWindow => offerWindow.Setup(offer));
    }
}