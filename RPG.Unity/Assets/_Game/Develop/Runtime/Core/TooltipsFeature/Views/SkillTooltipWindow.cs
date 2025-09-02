using System;
using PleasantlyGames.RPG.Runtime.Core.Cheats;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Views
{
    public class SkillTooltipWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField, Required] private ItemInfoView _itemInfoView;
        [SerializeField, Required] private ItemModifierView _ownedModifierView;
        [SerializeField, Required] private TextMeshProUGUI _description;
        [SerializeField, Required] private TextMeshProUGUI _cooldown;

        [Inject] private ItemSkillService _itemSkillService;
        [Inject] private ItemConfiguration _itemConfiguration;
        [Inject] private ITranslator _translator;
        [Inject] private SkillInventory _inventory;

        private SkillItem _item;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (_item != null) Clear();
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        public void Setup(SkillItem currentItem)
        {
            SetItem(currentItem);
        }

        private void Clear()
        {
            _item = null;
        }

        private void Redraw()
        {
            _ownedModifierView.Draw(_item.OwnedEffectType, _item.OwnedModifier);
            _ownedModifierView.HideDelta();
            
            _itemInfoView.Draw(_item);

            var skill = _itemSkillService.GetSkillByItem(_item);
            
            _description.SetText(skill.GetDescription());
            _cooldown.SetText($"{StringExtension.Instance.CutDouble(skill.GetCooldown())} {_translator.Translate(TranslationConst.ShortSecond)}");
        }

        private void SetItem(SkillItem item)
        {    
            if (_item != null) Clear(); 
            _item = item;
            Redraw();
        }
    }
}