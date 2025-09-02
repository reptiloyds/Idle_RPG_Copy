using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Cheats;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PleasantlyGames.RPG.Runtime.Utilities.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Skill.View
{
    public class SkillWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField, Required] private ItemInfoView _itemInfoView;
        [SerializeField, Required] private ItemModifierView _ownedModifierView;
        [SerializeField, Required] private BaseButton _equipButton;
        [SerializeField, Required] private TextMeshProUGUI _equipButtonText;
        [SerializeField, Required] private BaseButton _enhanceButton;
        [SerializeField, Required] private BaseButton _leftButton;
        [SerializeField, Required] private BaseButton _rightButton;
        [SerializeField, Required] private BaseButton _cheatButton;
        [SerializeField, Required] private TextMeshProUGUI _description;
        [SerializeField, Required] private TextMeshProUGUI _cooldown;

        private IReadOnlyList<SkillItem> _items;
        private SkillItem _item;

        [Inject] private ItemSkillService _itemSkillService;
        [Inject] private ItemConfiguration _itemConfiguration;
        [Inject] private ITranslator _translator;
        [Inject] private SkillInventory _inventory;
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
                _inventory.Remove(_item);
            else
                _inventory.Equip(_item);
            
            Close();
        }

        private void OnEnhanceClick() => 
            _inventory.Enhance(_item.Id);

        protected override void OnCloseClick()
        {
            Clear();
            base.OnCloseClick();
        }

        public void Setup(IReadOnlyList<SkillItem> items, SkillItem currentItem)
        {
            _items = items;
            SetItem(currentItem);
        }

        private void Clear()
        {
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

            var skill = _itemSkillService.GetSkillByItem(_item);
            
            _description.SetText(skill.GetDescription());
            _cooldown.SetText($"{StringExtension.Instance.CutDouble(skill.GetCooldown())} {_translator.Translate(TranslationConst.ShortSecond)}");
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

        private void SetItem(SkillItem item)
        {    
            if (_item != null) Clear(); 
            _item = item;
            _item.OnAmountChanged += OnItemChanged;
            _item.OnLevelUp += OnItemChanged;
            _item.OnEquip += OnItemChanged;
            _item.OnTakeOff += OnItemChanged;
            Redraw();
        }

        private void OnItemChanged(Item item)
        {
            Redraw();
        }
    }
}