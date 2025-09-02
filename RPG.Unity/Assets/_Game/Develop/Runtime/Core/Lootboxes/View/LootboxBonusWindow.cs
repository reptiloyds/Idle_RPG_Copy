using System;
using PleasantlyGames.RPG.Runtime.BonusAccess.View;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.PurchasePresentation;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.View
{
    public class LootboxBonusWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField, Required] private BonusAccessButton _bonusButton;
        [SerializeField, Required] private TextMeshProUGUI _currentItemsText;
        [SerializeField, Required] private TextMeshProUGUI _nextItemsCount;
        [SerializeField, Required] private TextMeshProUGUI _maxItemsCount;
        [SerializeField, Required] private PurchaseContentPresenter _itemsPresenter;
        [SerializeField] private string _nextItemsToken = "next_ad_items";
        [SerializeField] private string _maximum = "maximum";

        [Inject] private ITranslator _translator;
        [Inject] private LootboxService _service;

        private Lootbox _lootbox;

        protected override void Awake()
        {
            base.Awake();

            _bonusButton.OnExecuted += OnBonusExecuted;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _bonusButton.OnExecuted -= OnBonusExecuted;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        public void Setup(Lootbox lootbox)
        {
            if (_lootbox != lootbox)
            {
                _bonusButton.Clear();
                _bonusButton
                    .SetLabelText($"{_translator.Translate(TranslationConst.Receive)}")
                    .Build();
            }
            _lootbox = lootbox;
            _currentItemsText.SetText($"x{lootbox.BonusOpenHandler.Items}");
            
            if (lootbox.BonusOpenHandler.Items == lootbox.BonusOpenHandler.MaxItems) 
                _nextItemsCount.SetText($"{_translator.Translate(_nextItemsToken)} {lootbox.BonusOpenHandler.Items}");
            else
                _nextItemsCount.SetText($"{_translator.Translate(_nextItemsToken)} {lootbox.BonusOpenHandler.Items + 1}");
            _maxItemsCount.SetText($"({_translator.Translate(_maximum)} {lootbox.BonusOpenHandler.MaxItems})");

            var sprites = _lootbox.Sprites;
            var color = _lootbox.Color;
            _itemsPresenter.Redraw(sprites, color, PurchaseContentBackground.Glow);
        }

        private void OnBonusExecuted()
        {
            Close();
            _lootbox.BonusOpenHandler.HandleReward();
        }
    }
}