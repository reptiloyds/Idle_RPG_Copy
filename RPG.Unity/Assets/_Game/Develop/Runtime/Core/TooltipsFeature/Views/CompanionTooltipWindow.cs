using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Cheats;
using PleasantlyGames.RPG.Runtime.Core.Companion;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.UI;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Views
{
    public class CompanionTooltipWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        [SerializeField] private List<CompanionStatView> _statViews;
        [SerializeField, Required] private ItemInfoView _itemInfoView;
        [SerializeField, Required] private ItemModifierView _ownedModifierView;
        
        [Inject] private ITranslator _translator;
        [Inject] private CompanionSquad _companionSquad;
        [Inject] private CompanionInventory _inventory;

        private CompanionItem _item;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_item != null) Clear();
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);
        
        protected override void OnCloseClick()
        {
            Clear();
            base.OnCloseClick();
        }

        public void Setup(CompanionItem currentItem)
        {
            SetItem(currentItem);
        }

        private void Clear()
        {
            foreach (var statView in _statViews) 
                statView.Clear();
            
            _item = null;
        }

        private void Redraw()
        {
            _ownedModifierView.Draw(_item.OwnedEffectType, _item.OwnedModifier);
            _ownedModifierView.HideDelta();
            _itemInfoView.Draw(_item);
        }

        private void SetItem(CompanionItem item)
        {    
            if (_item != null) Clear(); 
            _item = item;
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
    }
}