using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Cheats;
using PleasantlyGames.RPG.Runtime.Core.Companion;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.UI;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Companion.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CompanionWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField] private List<CompanionStatView> _statViews;
        [SerializeField, Required] private ItemInfoView _itemInfoView;
        [SerializeField, Required] private ItemModifierView _ownedModifierView;
        [SerializeField, Required] private BaseButton _equipButton;
        [SerializeField, Required] private TextMeshProUGUI _equipButtonText;
        [SerializeField, Required] private BaseButton _enhanceButton;
        [SerializeField, Required] private BaseButton _leftButton;
        [SerializeField, Required] private BaseButton _rightButton;
        [SerializeField, Required] private BaseButton _cheatButton;

        private IReadOnlyList<CompanionItem> _items;
        private CompanionItem _item;
        
        [Inject] private ITranslator _translator;
        [Inject] private CompanionSquad _companionSquad;
        [Inject] private CompanionInventory _inventory;
        [Inject] private CheatService _cheatService;
        
        protected override void Awake()
        {
            base.Awake();
            _leftButton.OnClick += OnLeftClick;
            _rightButton.OnClick += OnRightClick;
            _enhanceButton.OnClick += OnEnhanceClick;
            _equipButton.OnClick += OnEquipClick;
            
            _cheatButton.gameObject.SetActive(_cheatService.IsEnabled);
            if (_cheatService.IsEnabled) 
                _cheatButton.OnClick += OnCheatClick;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_item != null) Clear();
            
            _leftButton.OnClick -= OnLeftClick;
            _rightButton.OnClick -= OnRightClick;
            _enhanceButton.OnClick -= OnEnhanceClick;
            _equipButton.OnClick -= OnEquipClick;
            
            if (_cheatService.IsEnabled) 
                _cheatButton.OnClick -= OnCheatClick;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);
        
        private void OnCheatClick() => 
            _cheatService.AppendItem(_item.Id);

        private void OnEquipClick()
        {
            if (_item.IsEquipped)
                _inventory.Remove(_item.Id);
            else
                _inventory.Equip(_item.Id);
            
            Close();
        }

        private void OnEnhanceClick() => 
            _inventory.Enhance(_item.Id);
        
        protected override void OnCloseClick()
        {
            Clear();
            base.OnCloseClick();
        }

        public void Setup(IReadOnlyList<CompanionItem> items, CompanionItem currentItem)
        {
            _items = items;
            SetItem(currentItem);
        }

        private void Clear()
        {
            foreach (var statView in _statViews) 
                statView.Clear();
            
            _item.OnAmountChanged -= OnItemChanged;
            _item.OnLevelUp -= OnItemChanged;
            _item.OnEquip -= OnItemChanged;
            _item.OnTakeOff -= OnItemChanged;
            _item = null;
        }

        private void Redraw()
        {
            _ownedModifierView.Draw(_item.OwnedEffectType, _item.OwnedModifier);
            _ownedModifierView.HideDelta();
            
            _itemInfoView.Draw(_item);
            _enhanceButton.SetInteractable(_item.CanEnhance);

            _equipButtonText.SetText(_item.IsEquipped
                ? _translator.Translate(TranslationConst.Remove)
                : _translator.Translate(TranslationConst.Equip));
            _equipButton.SetInteractable(_item.IsUnlocked);
        }
        
        private void OnRightClick()
        {
            var index = _items.IndexOf(_item);
            index++;
            if (index >= _items.Count) index = 0;

            SetItem(_items[index]);
        }

        private void OnLeftClick()
        {
            var index = _items.IndexOf(_item);
            index--;
            if (index < 0) index = _items.Count - 1;

            SetItem(_items[index]);
        }

        private void SetItem(CompanionItem item)
        {    
            if (_item != null) Clear(); 
            _item = item;
            _item.OnAmountChanged += OnItemChanged;
            _item.OnLevelUp += OnItemChanged;
            _item.OnEquip += OnItemChanged;
            _item.OnTakeOff += OnItemChanged;
            Redraw();

            var stats = _companionSquad.GetStats(item);
            var statViewIndex = 0;
            foreach (var stat in stats)
            {
                if (!item.PresentStats.Contains(stat.Type)) continue;
                _statViews[statViewIndex].Setup(stat);
                statViewIndex++;
                if(statViewIndex == _statViews.Count) break;
            }
        }

        private void OnItemChanged(Item item) => 
            Redraw();
    }
}