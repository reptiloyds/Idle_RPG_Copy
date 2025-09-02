using System;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Views
{
    public class StuffTooltipWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField, Required] private ItemInfoView _itemInfoView;
        [SerializeField, Required] private ItemModifierView _ownedModifierView;
        [SerializeField, Required] private ItemModifierView _equipmentModifierView;

        [Inject] private ItemConfiguration _itemConfiguration;
        [Inject] private ITranslator _translator;
        [Inject] private StuffInventory _inventory;

        private StuffItem _item;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (_item != null) Clear();
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        public void Setup(StuffItem currentItem)
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
            
            _equipmentModifierView.Draw(_item.EquippedEffectType, _item.EquippedModifier);
            _equipmentModifierView.DrawDelta(_item.EquippedModifier, _item?.EquippedModifier);

            _itemInfoView.Draw(_item);
        }

        private void SetItem(StuffItem item)
        {    
            if (_item != null) Clear(); 
            _item = item;
            Redraw();
        }
    }
}