using System;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SelectedStuffView : MonoBehaviour
    {
        [SerializeField, Required] private ItemInfoView _itemInfo;
        [SerializeField, Required] private BaseButton _equipButton;
        [SerializeField, Required] private BaseButton _enhanceButton;
        
        [SerializeField, Required] private ItemModifierView _ownedModifier;
        [SerializeField, Required] private ItemModifierView _equippedModifier;

        private StuffItem _equippedItem;
        private StuffItem _item;
        
        [Inject] private ITranslator _translator;

        public StuffItem Item => _item;
        public BaseButton EquipButton => _equipButton;
        
        public event Action OnEnhance;
        public event Action OnEquip;

        public void Initialize()
        {
            _translator.OnChangeLanguage += OnChangeLanguage;
            _equipButton.OnClick += OnEquipClick;
            _enhanceButton.OnClick += OnEnhanceClick;
        }

        private void OnDestroy()
        {
            if(_translator != null)
                _translator.OnChangeLanguage -= OnChangeLanguage;
            if (_item != null)
            {
                _item.OnAmountChanged -= OnItemChanged;
                _item.OnLevelUp -= OnItemChanged;
            }
            _equipButton.OnClick -= OnEquipClick;
            _enhanceButton.OnClick -= OnEnhanceClick;
        }

        private void OnItemChanged(Item item) => 
            Redraw();

        private void OnEnhanceClick() => 
            OnEnhance?.Invoke();

        private void OnEquipClick() => 
            OnEquip?.Invoke();

        private void OnChangeLanguage()
        {
            if(!gameObject.activeSelf) return;
            Redraw();
        }

        public void SetEquippedItem(StuffItem item) => 
            _equippedItem = item;

        public void Draw(StuffItem item)
        {
            if (_item != null)
            {
                _item.OnAmountChanged -= OnItemChanged;
                _item.OnLevelUp -= OnItemChanged;
            } 
            _item = item;
            _item.OnAmountChanged += OnItemChanged;
            _item.OnLevelUp += OnItemChanged;
            Redraw();
        }

        public void Redraw()
        {
            _itemInfo.Draw(_item);
            
            _equipButton.SetInteractable(_item.IsUnlocked && !_item.IsEquipped);
            _enhanceButton.SetInteractable(_item.CanEnhance);
            
            _ownedModifier.Draw(_item.OwnedEffectType, _item.OwnedModifier);
            _equippedModifier.Draw(_item.EquippedEffectType, _item.EquippedModifier);

            if (_equippedItem == _item)
            {
                _equippedModifier.HideDelta();
                return;
            }
            if (_equippedItem == null)
                _equippedModifier.DrawDelta(_item.EquippedModifier, null);
            else
                _equippedModifier.DrawDelta(_item.EquippedModifier, _equippedItem.EquippedModifier);
        }
    }
}