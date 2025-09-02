using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Type;
using PleasantlyGames.RPG.Runtime.Core.Characters.View.Evolution;
using PleasantlyGames.RPG.Runtime.Core.Deal.Controller;
using PleasantlyGames.RPG.Runtime.Core.Deal.View;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.View;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.ShopHub.Type;
using PleasantlyGames.RPG.Runtime.Core.ShopHub.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    public class CharacterWindow : BaseWindow
    {
        [SerializeField] private CharacterPresenter _characterPresenter;
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private CharacterCard _characterCardTemplate;
        [SerializeField] private BaseButton _equipButton;
        [SerializeField] private BaseButton _evolveButton;
        [SerializeField] private ObtainCharacterButton _obtainButton;
        [SerializeField] private PeriodicRewardCharacterButton _periodicRewardButton;
        [SerializeField] private CharacterBranchMarkHandler _branchMarkHandler;

        [SerializeField] private ResourceDealView _experienceDealView;
        [SerializeField] private ResourceDealView _purchaseDealView;
        [SerializeField] private GameObject _maxObject;

        private ResourceDealController _experienceDealController;
        private ResourceDealController _purchaseDealController;

        private readonly List<CharacterCard> _views = new();
        private Branch _branch;

        [Inject] private ResourceService _resourceService;
        [Inject] private MessageBuffer _messageBuffer;
        [Inject] private ITranslator _translator;
        [Inject] private BranchService _branchService;
        [Inject] private CharacterService _characterService;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IWindowService _windowService;
        [Inject] private ProductService _productService;

        private CharacterCard _selectedCard;
        private CharacterCard _equippedCard;

        public IReadOnlyList<CharacterCard> Views => _views;

        protected override void Awake()
        {
            base.Awake();
            _experienceDealController =
                new ResourceDealController(_experienceDealView, _resourceService, _messageBuffer, _translator);
            _purchaseDealController =
                new ResourceDealController(_purchaseDealView, _resourceService, _messageBuffer, _translator);

            _equipButton.OnClick += OnEquipClick;
            _evolveButton.OnClick += OnEvolveClick;

            _purchaseDealController.OnSuccess += OnResourcePurchaseSuccess;
            _experienceDealController.OnSuccess += OnAddExperienceSuccess;
            _characterService.OnLevelUp += OnLevelUp;
            _characterService.OnEvolved += OnEvolved;
            _obtainButton.OnClick += OnObtainClick;
            _periodicRewardButton.OnClick += OnPeriodicRewardClick;

            RedrawExperienceDealView();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _equipButton.OnClick -= OnEquipClick;
            _evolveButton.OnClick -= OnEvolveClick;

            _purchaseDealController.OnSuccess -= OnResourcePurchaseSuccess;
            _experienceDealController.OnSuccess -= OnAddExperienceSuccess;
            _characterService.OnLevelUp -= OnLevelUp;
            _characterService.OnEvolved -= OnEvolved;
            _obtainButton.OnClick -= OnObtainClick;
            _periodicRewardButton.OnClick -= OnPeriodicRewardClick;

            foreach (var view in _views)
                view.OnClick -= OnCardClick;
        }

        public CharacterCard GetViewByModel(Character model)
        {
            foreach (var view in _views)
                if(view.Character == model) return view;
            return null;
        }

        private async void OnPeriodicRewardClick()
        {
            _windowService.CloseAll();
            await _windowService.OpenAsync<DailyLoginRewardWindow>();
        }

        private async void OnObtainClick(Product product)
        {
            _windowService.CloseAll();
            var shopHubWindow = await _windowService.OpenAsync<ShopHubWindow>();
            shopHubWindow.Select(ShopElement.PermanentProduct);
            if (shopHubWindow.PermanentProductCollectionView.MergedViews.TryGetValue(product.MergedKey, out var mergedView))
                mergedView.Select(product);
        }

        private async void OnEvolveClick()
        {
            var window = await _windowService.OpenAsync<CharacterEvolutionWindow>();
            window.Setup(_selectedCard.Character);
        }

        private void OnLevelUp(Character character) =>
            RedrawDealViews();

        private void OnResourcePurchaseSuccess(ResourceDealController dealController)
        {
            _characterService.Own(_selectedCard.Character);
            RedrawEquipButton(_selectedCard.Character);
            RedrawDealViews();
        }

        private void OnAddExperienceSuccess(ResourceDealController dealController)
        {
            _characterService.AddExperience(_selectedCard.Character);
            RedrawDealViews();
        }

        private void OnEvolved(Character obj) =>
            RedrawDealViews();

        private void OnEquipClick()
        {
            _equippedCard.Unequipped();
            _equippedCard = _selectedCard;
            _equippedCard.Equipped();
            _characterService.SwitchCharacter(_equippedCard.Character.Id);
            RedrawEquipButton(_equippedCard.Character);
        }

        public override void Open()
        {
            base.Open();
            RedrawCards();
            RedrawDealViews();
            _branchMarkHandler.Enable();
        }

        public override void Close()
        {
            base.Close();
            _branchMarkHandler.Disable();
        }

        private void RedrawCards()
        {
            var selectedBranch = _branchService.GetSelectedBranch();
            // if (_branch != null && _branch.Id == selectedBranch.Id) return;
            
            _branch = selectedBranch;
            var characterId = 0;
            var characters = _characterService.GetCharactersForBranch(_branch.Id);
            foreach (var character in characters)
            {
                if(_branchService.IsDefaultCharacter(character.Id) && !character.IsOwned) continue;
                var card = GetCard(characterId);
                card.gameObject.SetActive(true);
                card.Setup(character);
                if (character.Id == _branch.CharacterId)
                {
                    _equippedCard = card;
                    SelectCard(card);
                    card.Equipped();
                }
                else
                {
                    card.Unselected();
                    card.Unequipped();
                }

                characterId++;
            }

            for (var i = characterId; i < _views.Count; i++)
            {
                _views[i].Clear();
                _views[i].gameObject.SetActive(false);
            }
        }

        private CharacterCard GetCard(int id)
        {
            if (id < _views.Count) return _views[id];

            var item = _objectResolver.Instantiate(_characterCardTemplate, _cardContainer);
            item.SetupButtonId($"Character_{id + 1}");
            _views.Add(item);
            item.OnClick += OnCardClick;
            return item;
        }

        private void OnCardClick(CharacterCard card) =>
            SelectCard(card);

        private void SelectCard(CharacterCard card)
        {
            if (_selectedCard != null)
                _selectedCard.Unselected();

            _selectedCard = card;
            _selectedCard.Selected();

            _characterPresenter.Setup(_selectedCard.Character);

            RedrawEquipButton(card.Character);
            RedrawDealViews();
        }

        private void RedrawEquipButton(Character character)
        {
            _equipButton.gameObject.SetActive(character.IsOwned);
            _equipButton.SetInteractable(_branch.CharacterId != character.Id && !character.IsEquipped);
        }

        private void RedrawDealViews()
        {
            if (_selectedCard.Character.IsMaxEnhanced)
            {
                ShowMax();
                return;
            }

            switch (_selectedCard.Character.IsOwned)
            {
                case false when _selectedCard.Character.OwnType == CharacterOwnType.Resource:
                    ShowResourcePurchase();
                    return;
                case false when _selectedCard.Character.OwnType == CharacterOwnType.InApp:
                    ShowObtainButton();
                    return;
                case false when _selectedCard.Character.OwnType == CharacterOwnType.PeriodicReward:
                    ShowPeriodicRewardButton();
                    return;
                case true when _selectedCard.Character.IsEvolutionReady():
                    ShowEvolutionButton();
                    return;
                case true when !_selectedCard.Character.IsEvolutionReady():
                    ShowExperienceButton();
                    return;
            }
        }

        private void ShowResourcePurchase()
        {
            _purchaseDealView.gameObject.SetActive(true);
            _evolveButton.gameObject.SetActive(false);
            _experienceDealView.gameObject.SetActive(false);
            _maxObject.gameObject.SetActive(false);
            RedrawPurchaseDealView();
        }

        private void RedrawPurchaseDealView()
        {
            _purchaseDealController.ClearPrice();
            var priceList = _selectedCard.Character.PurchasePrice;
            foreach (var priceStruct in priceList)
                _purchaseDealController.AddPrice(priceStruct.Type, priceStruct.GetValue());
            _purchaseDealController.BuildPrice();
        }

        private void ShowObtainButton()
        {
            _purchaseDealView.gameObject.SetActive(false);
            _evolveButton.gameObject.SetActive(false);
            _experienceDealView.gameObject.SetActive(false);
            _maxObject.gameObject.SetActive(false);
            _obtainButton.gameObject.SetActive(true);
            _periodicRewardButton.gameObject.SetActive(false);
            
            foreach (var product in _productService.Products)
            {
                if(product.Rewards.Character == null) continue;
                if (product.Rewards.Character.Id != _selectedCard.Character.Id) continue;
                _obtainButton.Setup(product);
                break;
            }
        }

        private void ShowPeriodicRewardButton()
        {
            _purchaseDealView.gameObject.SetActive(false);
            _evolveButton.gameObject.SetActive(false);
            _experienceDealView.gameObject.SetActive(false);
            _maxObject.gameObject.SetActive(false);
            _obtainButton.gameObject.SetActive(false);
            _periodicRewardButton.gameObject.SetActive(true);
        }

        private void ShowEvolutionButton()
        {
            _purchaseDealView.gameObject.SetActive(false);
            _evolveButton.gameObject.SetActive(true);
            _experienceDealView.gameObject.SetActive(false);
            _maxObject.gameObject.SetActive(false);
            _obtainButton.gameObject.SetActive(false);
            _periodicRewardButton.gameObject.SetActive(false);
        }

        private void ShowExperienceButton()
        {
            _purchaseDealView.gameObject.SetActive(false);
            _evolveButton.gameObject.SetActive(false);
            _experienceDealView.gameObject.SetActive(true);
            _maxObject.gameObject.SetActive(false);
            _obtainButton.gameObject.SetActive(false);
            _periodicRewardButton.gameObject.SetActive(false);
        }

        private void ShowMax()
        {
            _purchaseDealView.gameObject.SetActive(false);
            _evolveButton.gameObject.SetActive(false);
            _experienceDealView.gameObject.SetActive(false);
            _maxObject.gameObject.SetActive(true);
            _obtainButton.gameObject.SetActive(false);
            _periodicRewardButton.gameObject.SetActive(false);
        }

        private void RedrawExperienceDealView()
        {
            _experienceDealController.ClearPrice();
            _experienceDealController.AddPrice(ResourceType.ExpShard, 1);
            _experienceDealController.BuildPrice();
        }
    }
}