using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Deal.Model;
using PleasantlyGames.RPG.Runtime.Core.Deal.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Deal.Controller
{
    public class ResourceDealController
    {
        private readonly ResourceDealView _view;
        private readonly ITranslator _translator;
        private readonly MessageBuffer _messageBuffer;
        private readonly ResourceService _resourceService;

        private readonly List<ResourcePrice> _prices = new();
        private int _priceId = 0;
        private Func<bool> _interactionCondition;

        public ResourceDealView View => _view;
        public event Action<ResourceDealController> OnSuccess;

        public ResourceDealController(ResourceDealView view, ResourceService resourceService, MessageBuffer messageBuffer, ITranslator translator)
        {
            _view = view;
            _resourceService = resourceService;
            _messageBuffer = messageBuffer;
            _translator = translator;
            
            _view.OnEnableEvent += OnViewEnable;

            EnableInternalInteraction();
        }

        public void SetInteractionCondition(Func<bool> condition) => 
            _interactionCondition = condition;

        private void OnViewEnable() => 
            Redraw();

        public void ClearPrice()
        {
            foreach (var price in _prices)
            {
                price.ResourceModel.OnChange -= OnResourceChange;
                price.Clear();
            } 
            _priceId = 0;
        }

        public ResourceDealController AddPrice(ResourceType resourceType, BigDouble.Runtime.BigDouble value)
        {
            var resource = _resourceService.GetResource(resourceType);
            resource.OnChange += OnResourceChange;
            var price = GetPrice(_priceId);
            price.Setup(resource, value);
            _priceId++;

            return this;
        }

        private void OnResourceChange()
        {
            if(!_view.gameObject.activeSelf) return;
            Redraw();
        }

        private void Redraw()
        {
            if(_view.ShowTotalValues)
                _view.RedrawPrice(_prices);
            UpdateInteraction();
        }

        private ResourcePrice GetPrice(int id)
        {
            if (id < _prices.Count) return _prices[id];
            
            var price = new ResourcePrice();
            _prices.Add(price);
            return price;

        }

        public void BuildPrice()
        {
            _view.RedrawPrice(_prices);
            UpdateInteraction();
        }

        private void OnClick()
        {
            foreach (var price in _prices)
            {
                if(price.IsEmpty) break;
                price.ResourceModel.Spend(price.Amount);
            }
            
            OnSuccess?.Invoke(this);
        }

        private void OnFailClick()
        {
            if(IsEnough()) return;
            foreach (var price in _prices)
            {
                if(price.IsEmpty) break;
                _messageBuffer.Send(_translator.Translate($"{TranslationConst.NotEnoughPrefix}{price.Type}"));   
            }
        }

        public void DisableInternalInteraction()
        {
            _view.Button.OnClick -= OnClick;
            _view.Button.OnFailClick -= OnFailClick;
        }

        public void EnableInternalInteraction()
        {
            _view.Button.OnClick += OnClick;
            _view.Button.OnFailClick += OnFailClick;
        }

        private void UpdateInteraction()
        {
            if (_interactionCondition != null) 
                _view.Button.SetInteractable(IsEnough() && _interactionCondition());
            else
                _view.Button.SetInteractable(IsEnough());
        }

        private bool IsEnough()
        {
            foreach (var price in _prices)
            {
                if (!price.IsEnough) return false;
                if (price.IsEmpty) break;
            }

            return true;
        }
    }
}