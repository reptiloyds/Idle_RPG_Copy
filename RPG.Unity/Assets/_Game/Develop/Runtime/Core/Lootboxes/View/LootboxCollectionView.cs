using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.ShopHub.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.View
{
    public class LootboxCollectionView : ShopElementView
    {
        [SerializeField, Required] private RectTransform _container;
        [SerializeField, Required] private LootboxView _lootboxPrefab;

        private readonly List<LootboxView> _views = new(16);
        
        [Inject] private IObjectResolver _resolver;
        [Inject] private LootboxService _service;
        [Inject] private IWindowService _windowService;

        private void Awake() => 
            CreateViews();

        private void OnDestroy()
        {
            foreach (var view in _views)
            {
                view.OnOpenClicked -= OnOpenClicked;
                view.OnBonusClicked -= OnBonusClicked;
            }
        }

        public LootboxView GetViewByModel(Lootbox lootbox)
        {
            foreach (var view in _views)
                if (view.Model == lootbox) return view;

            return null;
        }

        private void CreateViews()
        {
            foreach (var model in _service.Lootboxes) 
                CreateViewFor(model);
        }

        private void CreateViewFor(Lootbox model)
        {
            var view = _resolver.Instantiate(_lootboxPrefab, _container);
            _views.Add(view);
            view.Setup(model);
            view.OnOpenClicked += OnOpenClicked;
            view.OnBonusClicked += OnBonusClicked; 
        }

        private void OnOpenClicked(LootboxView view, int amount) => 
            _service.Purchase(view.Model, amount);

        private async void OnBonusClicked(LootboxView view)
        {
            var window = await _windowService.OpenAsync<LootboxBonusWindow>();
            window.Setup(view.Model);
        }
    }
}