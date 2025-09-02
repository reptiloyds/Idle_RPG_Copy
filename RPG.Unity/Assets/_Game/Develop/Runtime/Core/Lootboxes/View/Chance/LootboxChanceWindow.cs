using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.View.Chance
{
    public class LootboxChanceWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private BaseButton _leftButton;
        [SerializeField] private BaseButton _rightButton;
        [SerializeField] private LootboxChanceView _viewPrefab;
        [SerializeField] private RectTransform _container;

        private LootboxSheet _lootboxSheet;
        private readonly List<LootboxChanceView> _views = new();

        private string _lootboxId;
        private int _level;
        
        [Inject] private BalanceContainer _balanceContainer;
        [Inject] private ITranslator _translator;
        [Inject] private ItemConfiguration _itemConfiguration;

        protected override void Awake()
        {
            base.Awake();
            
            _lootboxSheet = _balanceContainer.Get<LootboxSheet>();
            
            _leftButton.OnClick += OnLeftButtonClick;
            _rightButton.OnClick += OnRightButtonClick;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _leftButton.OnClick -= OnLeftButtonClick;
            _rightButton.OnClick -= OnRightButtonClick;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        private void OnLeftButtonClick() => 
            Setup(_level - 1, _lootboxId);

        private void OnRightButtonClick() => 
            Setup(_level + 1, _lootboxId);

        public void Setup(int level, string lootboxId)
        {
            if (!_lootboxSheet.TryGetValue(lootboxId, out var lootboxConfig)) return;

            _lootboxId = lootboxId;
            
            if (level - 1 >= lootboxConfig.Arr.Count) level = lootboxConfig.Arr.Count;
            else if (level < 1) level = 1;

            _level = level;

            var levelConfig = lootboxConfig.Arr[_level - 1];
            
            _levelText.SetText($"{_translator.Translate(TranslationConst.Level)} {_level}");

            if (_level == 1)
            {
                _leftButton.gameObject.SetActive(false);
                _rightButton.gameObject.SetActive(true);
            }
            else if (_level == lootboxConfig.Arr.Count)
            {
                _leftButton.gameObject.SetActive(true);
                _rightButton.gameObject.SetActive(false);   
            }
            else
            {
                _leftButton.gameObject.SetActive(true);
                _rightButton.gameObject.SetActive(true);
            }

            var rarities = Enum.GetValues(typeof(ItemRarityType))
                .Cast<ItemRarityType>()
                .Except(new[] { ItemRarityType.None })
                .ToList();
            
            var viewId = 0;
            foreach (var rarity in rarities)
            {
                if (!levelConfig.Chances.TryGetValue(rarity, out float chance)) continue;

                var view = GetView(viewId);
                view.Enable();
                _itemConfiguration.GetColor(rarity, out var color);
                view.Setup(_translator.Translate(rarity.ToString()), chance.ToString("F4"), color);
                viewId++;
            }
            
            for (; viewId < _views.Count; viewId++) 
                _views[viewId].Disable();
        }

        private LootboxChanceView GetView(int id)
        {
            if (id >= _views.Count)
            {
                var view = Instantiate(_viewPrefab, _container);
                _views.Add(view);
                return view;
            }

            return _views[id];
        }
    }
}