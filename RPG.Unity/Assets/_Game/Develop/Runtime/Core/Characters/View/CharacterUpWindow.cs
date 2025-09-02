using System;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CharacterUpWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private WindowTweens.ScaleGroup[] _scaleGroups;
        [SerializeField] private List<ParticleImage> _particles;
        
        [SerializeField, BoxGroup("Character")] private Image _characterImage;
        [SerializeField, BoxGroup("Character")] private TextMeshProUGUI _levelCharName;
        [SerializeField, BoxGroup("Character")] private GameObject _evolutionNameObject;
        [SerializeField, BoxGroup("Character")] private TextMeshProUGUI _evolutionCharName;
        [SerializeField, BoxGroup("Character")] private TextMeshProUGUI _evolutionAmount;
        
        [SerializeField, BoxGroup("Bonus")] private TextMeshProUGUI _bonusUpgradeText;
        [SerializeField, BoxGroup("Bonus")] private Image _bonusImage;
        [SerializeField, BoxGroup("Bonus")] private Image _bonusBackImage;
        [SerializeField, BoxGroup("Bonus")] private TextMeshProUGUI _bonusStatText;
        [SerializeField, BoxGroup("Bonus")] private Sprite _defaultBackground;
        [SerializeField, BoxGroup("Bonus")] private Sprite _evolutionBackground;
        [SerializeField, BoxGroup("Bonus")] private GameObject _starObject;
        [SerializeField, BoxGroup("Bonus")] private Color _bonusColor;

        [Inject] private ITranslator _translator;

        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleGroupTween(_scaleGroups, UseUnscaledTime, callback);

        public override void Open()
        {
            base.Open();

            foreach (var particle in _particles) 
                particle.Play();
        }

        public void Setup(Character character, ICharacterBonus bonus)
        {
            _characterImage.sprite = character.Sprite;
            _bonusUpgradeText.SetText(bonus.Level == 1 
                ? _translator.Translate(TranslationConst.BonusUnlocked) 
                : _translator.Translate(TranslationConst.BonusImproved));
            _bonusImage.sprite = bonus.Sprite;
            _bonusStatText.SetText($"{bonus.GetStatName()} <color={_bonusColor.ToHexString()}>{bonus.GetStatValue(bonus.Level)}</color>");
            var isEvolution = bonus.ConditionType == BonusConditionType.Evolution;

            _bonusBackImage.sprite = isEvolution ? _evolutionBackground : _defaultBackground;
            _starObject.SetActive(isEvolution);

            if (character.Evolution > 0)
            {
                _levelCharName.gameObject.SetActive(false);
                _evolutionNameObject.SetActive(true);
                _evolutionAmount.SetText(character.Evolution.ToString());
                _evolutionCharName.SetText($"{TranslationConst.LevelPrefixCaps}{character.Level} {character.FormattedName}");
            }
            else
            {
                _levelCharName.gameObject.SetActive(true);
                _evolutionNameObject.SetActive(false);
                _levelCharName.SetText($"{TranslationConst.LevelPrefixCaps}{character.Level} {character.FormattedName}");
            }
        }
    }
}