using System;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class SlotView<T> : MonoBehaviour where T : Slot
    {
        [SerializeField, Required, BoxGroup("Main")] protected BaseButton _baseButton;
        [SerializeField, Required, BoxGroup("Main")] protected GameObject _blockObject;
        [SerializeField, Required, BoxGroup("Main")] protected GameObject _emptySlot;
        
        [SerializeField, Required, FoldoutGroup("Item")] protected GameObject _itemVisual;
        [SerializeField, Required, FoldoutGroup("Item")] protected Image _itemImage;
        [SerializeField, Required, FoldoutGroup("Item")] protected Image _itemRarityImage;
        [SerializeField, Required, FoldoutGroup("Item")] protected bool _showLevel = false;
        [SerializeField, Required, FoldoutGroup("Item"), HideIf("@this._showLevel == false")]
        private TextMeshProUGUI _itemLevelText;
        
        [Inject] private MessageBuffer _messageBuffer;

        private T _slot;

        public event Action<SlotView<T>> SuccessClick;
        public T Slot => _slot;
        public BaseButton Button => _baseButton;
        
        private void Awake() => 
            _baseButton.OnClick += OnClick;

        private void OnDestroy()
        {
            ClearModel();
            _baseButton.OnClick -= OnClick;
        }
        
        private void OnClick()
        {
            if (!_slot.IsUnlocked)
            {
                _messageBuffer.Send(_slot.Condition);
                return;
            }
            
            Click();
        }

        protected virtual void Click() => 
            SuccessClick?.Invoke(this);

        private void ClearModel()
        {
            if (_slot != null)
            {
                if (_slot.BaseItem != null) 
                    _slot.BaseItem.OnLevelUp -= OnItemLevelUp;
                _slot.OnItemChanged -= Redraw;
                _slot.OnUnlocked -= OnUnlock;
                _slot = null;
            } 
        }

        public void SetModel(T slot)
        {
            ClearModel();
            
            _slot = slot;
            _slot.OnItemChanged += Redraw;
            _slot.OnUnlocked += OnUnlock;
            _slot.OnEquip += OnEquipItem;
            _slot.OnRemove += OnRemoveItem;
            if(_slot.BaseItem != null)
                _slot.BaseItem.OnLevelUp += OnItemLevelUp;
        }

        private void OnEquipItem(Item item) => 
            item.OnLevelUp += OnItemLevelUp;

        private void OnRemoveItem(Item item) => 
            item.OnLevelUp -= OnItemLevelUp;

        private void OnUnlock(IUnlockable unlockable) => 
            Redraw();

        public void Redraw()
        {
            if(_slot == null) return;
            
            if (_slot.BaseItem != null)
                ShowItem();
            else
                HideItem();
            
            _blockObject.gameObject.SetActive(!_slot.IsUnlocked);
        }

        public void Enable() => gameObject.SetActive(true);

        public void Disable() => gameObject.SetActive(false);

        protected virtual void ShowItem()
        {
            _emptySlot.SetActive(false);
            _itemImage.sprite = _slot.BaseItem.Sprite;
            RedrawLevel();
            
            _itemRarityImage.color = _slot.BaseItem.RarityColor;
            _itemVisual.SetActive(true);
        }

        private void OnItemLevelUp(Item item) =>
            RedrawLevel();

        private void RedrawLevel()
        {
            if (_showLevel) 
                _itemLevelText.SetText($"{TranslationConst.LevelPrefixCaps} {_slot.BaseItem.Level}");
        }

        protected virtual void HideItem()
        {
            _emptySlot.SetActive(true);
            _itemVisual.SetActive(false);
        }
    }
}