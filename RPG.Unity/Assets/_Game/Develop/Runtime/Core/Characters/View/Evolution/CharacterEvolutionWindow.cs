using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Deal.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View.Evolution
{
    public class CharacterEvolutionWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private Image _characterImage;
        [SerializeField] private TextMeshProUGUI _characterNameText;
        [SerializeField] private TextMeshProUGUI _levelCapText;
        [SerializeField] private TextMeshProUGUI _effectValueText;
        [SerializeField] private EvolutionPrice _price;
        
        [SerializeField] private BaseButton _evolveButton;

        [Inject] private ITranslator _translator;
        [Inject] private CharacterService _characterService;
        [Inject] private ResourceService _resourceService;

        private Character _character;
        
        private List<ResourcePriceStruct> _prices;

        protected override void Awake()
        {
            base.Awake();
            _evolveButton.OnClick += Evolve;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _evolveButton.OnClick -= Evolve;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        public void Setup(Character character)
        {
            _character = character;
            SetupCharacter();
            SetupLevelCap();
            SetupEffect();
            SetupPrice();

            var prices = character.GetEvolutionPrice();
            bool isEnough = true;
            foreach (var price in prices)
            {
                isEnough = _resourceService.IsEnough(price.Type, price.GetValue());
                if(!isEnough) break;
            }
            _evolveButton.SetInteractable(isEnough);
        }

        private void SetupCharacter()
        {
            _characterImage.sprite = _character.Sprite;
            _characterNameText.SetText(_character.FormattedName);
        }

        private void SetupPrice()
        {
            _prices = _character.GetEvolutionPrice();
            _price.Setup(_prices);
        }

        private void SetupLevelCap() => 
            _levelCapText.SetText(string.Format(_translator.Translate(TranslationConst.MaxLevelDelta),
                _character.GetCurrentLevelCap(), _character.GetNextLevelCap()));

        private void SetupEffect()
        {
            var bonuses = _character.GetBonuses();
            string evolveEffect = string.Empty;
            foreach (var bonus in bonuses)
            {
                if(bonus.ConditionType != BonusConditionType.Evolution) continue;
                if (!string.IsNullOrEmpty(evolveEffect)) evolveEffect += ", ";
                evolveEffect += $"{bonus.GetStatName()} {bonus.GetStatValue(bonus.Level + 1)}";
            }
            _effectValueText.SetText(string.Format(_translator.Translate(TranslationConst.EvolveEffect), evolveEffect));
        }

        private void Evolve()
        {
            _characterService.Evolve(_character);
            Close();
        }
    }
}