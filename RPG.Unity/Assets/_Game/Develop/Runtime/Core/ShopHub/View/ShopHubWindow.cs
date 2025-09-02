using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.View;
using PleasantlyGames.RPG.Runtime.Core.Products.View;
using PleasantlyGames.RPG.Runtime.Core.Products.View.Periodic;
using PleasantlyGames.RPG.Runtime.Core.ShopHub.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.ShopHub.View
{
    public class ShopHubWindow : BaseWindow
    {
        [SerializeField] private ShopElement _defaultElement = ShopElement.Lootbox;
        [SerializeField] private List<ShopButton> _buttons;
        [SerializeField] private LootboxCollectionView _lootboxCollectionView;
        [SerializeField] private PermanentProductCollectionView _permanentProductCollectionView;
        [SerializeField] private PeriodicProductCollectionsView _periodicProductCollectionsView;

        private readonly List<ShopElementView> _views = new();
        private ShopElement _element;

        public LootboxCollectionView LootboxCollectionView => _lootboxCollectionView;
        public PermanentProductCollectionView PermanentProductCollectionView => _permanentProductCollectionView;
        public PeriodicProductCollectionsView PeriodicProductCollectionsView => _periodicProductCollectionsView;
        
        protected override void Awake()
        {
            base.Awake();
            
            _views.Add(_lootboxCollectionView);
            _views.Add(_permanentProductCollectionView);
            _views.Add(_periodicProductCollectionsView);
            foreach (var button in _buttons) 
                button.OnSelect += OnButtonSelect;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (var button in _buttons) 
                button.OnSelect -= OnButtonSelect;
        }

        private void OnButtonSelect(ShopElement element) => 
            Select(element);

        public override void Open()
        {
            base.Open();

            //if (_element == ShopElement.None) 
            Select(_defaultElement);
        }

        public void Select(ShopElement element)
        {
            if(_element == element) return;
            
            _element = element;

            foreach (var button in _buttons)
            {
                var view = GetView(button.Element);
                if (button.Element != _element)
                {
                    view.Hide();
                    button.Unselect();
                }
                else
                {
                    view.Show();
                    button.Select();
                }
            }
        }

        private ShopElementView GetView(ShopElement type)
        {
            foreach (var view in _views)
                if (view.Type == type) return view;

            return null;
        }

        public BaseButton GetButton(ShopElement lootbox)
        {
            foreach (var button in _buttons)
                if (button.Element == lootbox)
                    return button;

            return null;
        }
    }
}