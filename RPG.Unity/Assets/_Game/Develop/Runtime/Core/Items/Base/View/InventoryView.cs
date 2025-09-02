using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View.Enhance;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class InventoryView : MonoBehaviour
    {
        [SerializeField, Required, BoxGroup("Main")] private Transform _itemContainer;
        [SerializeField, Required, BoxGroup("Main")] private ItemView _itemPrefab;
        [SerializeField, Required, BoxGroup("Main")] private BaseButton _enhanceAllButton;
        
        [SerializeField, Required, BoxGroup("SingleItem")] private GameObject _singleItemContainer;
        [SerializeField, Required, BoxGroup("SingleItem")] private ItemView _itemView;
        [SerializeField, Required, BoxGroup("SingleItem")] private BaseButton _cancelSingleItemButton;
        
        private ItemView _selectedItemView;
        private Item _selectedItem;
        
        private readonly List<ItemView> _views = new();
        private bool _equippedSignIsActive = true;
        
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IWindowService _windowService;

        private Item _singleItem;
        
        public Item SelectedItem => _selectedItem;
        public Item SingleItem => _singleItem;
        public BaseButton EnhanceButton => _enhanceAllButton;
        public List<ItemView> Views => _views;
        public bool IsSingleItemEnable { get; private set; }
        
        public event Action EnhanceAll;
        public event Action<Item> ItemUnlock;
        public event Action<ItemView> ItemClick;
        public event Action CancelSingleItem;
        
        private void CancelSingleItemClick()
        {
            CancelSingleItem?.Invoke();
            DisableSingleItem();
        }

        public void EnableSingleItem(Item item)
        {
            _singleItem = item;
            _itemView.SetModel(_singleItem);
            _itemView.Unselect();
            _itemView.HideEquippedObject();
            _singleItemContainer.SetActive(true);
            IsSingleItemEnable = true;
            _itemContainer.gameObject.SetActive(false);
        }

        public void DisableSingleItem()
        {
            ClearSingleItem();
            _singleItemContainer.SetActive(false);
            IsSingleItemEnable = false;
            _itemContainer.gameObject.SetActive(true);
        }
        
        private void ClearSingleItem()
        {
            if (_singleItem == null) return;
            _itemView.ClearModel();
            _singleItem = null;
        }

        public void Initialize()
        {
            _enhanceAllButton.OnClick += OnEnhanceClick;
            _cancelSingleItemButton.OnClick += CancelSingleItemClick;
        }

        private void OnDestroy()
        {
            _enhanceAllButton.OnClick -= OnEnhanceClick;
            _cancelSingleItemButton.OnClick -= CancelSingleItemClick;
            
            foreach (var item in _views) 
                item.OnClick -= OnItemClick;

            Clear();
        }

        private async void OnEnhanceClick()
        {
            var window = await _windowService.OpenAsync<EnhanceItemWindow>(SetupEnhanceWindow);
            
            foreach (var itemView in _views)
            {
                if(itemView.Model == null) continue;
                if(!itemView.Model.CanEnhance) continue;
                window.AddItem(itemView.Model);
            }
            
            RedrawEnhanceButton();
            EnhanceAll?.Invoke();
        }

        private void SetupEnhanceWindow(EnhanceItemWindow window)
        {
            foreach (var itemView in _views)
            {
                if(itemView.Model == null) continue;
                if(!itemView.Model.CanEnhance) continue;
                window.AddItem(itemView.Model);
            }
        }

        public void RedrawEnhanceButton()
        {
            bool canEnhance = false;
            foreach (var view in _views)
            {
                if(view.Model == null) continue;
                if(!view.Model.CanEnhance) continue;
                canEnhance = true;
                break;
            }
            _enhanceAllButton.SetInteractable(canEnhance);
        }

        public void Setup(Item item, int viewId, string buttonId)
        {
            var view = GetView(viewId);
            view.SetModel(item);
            view.SetButtonId(buttonId);
            item.OnUnlock += OnItemUnlock;
            view.Enable();
            view.Unselect();
            
            if(_equippedSignIsActive)
                view.ShowEquippedObject();
            else
                view.HideEquippedObject();
        }
        
        public ItemView GetViewByModel(Item item)
        {
            foreach (var view in _views)
                if (view.Model == item) return view;

            return null;
        }

        private void OnItemUnlock(Item item) => 
            ItemUnlock?.Invoke(item);

        public void Select(Item item)
        {
            _selectedItem = item;
            _selectedItemView?.Unselect();
            foreach (var itemView in _views)
            {
                if(itemView.Model != item) continue;
                _selectedItemView = itemView;
                _selectedItemView.Select();
                break;
            }
        }
            
        private ItemView GetView(int id)
        {
            if (id >= _views.Count)
            {
                var item = _objectResolver.Instantiate(_itemPrefab, _itemContainer);
                _views.Add(item);
                item.OnClick += OnItemClick;
                return item;
            }
            return _views[id];
        }

        private void OnItemClick(ItemView item) => 
            ItemClick?.Invoke(item);

        public void HideEquippedSign()
        {
            _equippedSignIsActive = false;
            foreach (var view in _views) 
                view.HideEquippedObject();
        }

        public void ShowEquippedSign()
        {
            _equippedSignIsActive = true;
            foreach (var view in _views) 
                view.ShowEquippedObject();
        }

        public void DisableEmpty()
        {
            foreach (var view in _views)
            {
                if(view.Model == null)
                    view.Disable();
            }
        }

        public void Clear()
        {
            foreach (var item in _views)
            {
                if (item.Model == null) continue;
                item.Model.OnUnlock -= OnItemUnlock;
                item.ClearModel();
            } 
        }

        public void SortByRarity()
        {
            _views.Sort((x, y) =>
            {
                if (x.Model.Rarity == y.Model.Rarity) return 0;
                return x.Model.Rarity > y.Model.Rarity ? 1 : -1;
            });
            for (var i = 0; i < _views.Count; i++) 
                _views[i].transform.SetSiblingIndex(i);
        }
    }
}