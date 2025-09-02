using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.UI.ScreenClamper;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CharacterBonusHintView : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private ScreenFitter _screenFitter;
        [SerializeField] private GameObject _blockVisual;
        [SerializeField, BoxGroup("Bonus")] private GameObject _bonusEffect;
        [SerializeField, BoxGroup("Bonus")] private TextMeshProUGUI _bonusText;
        [SerializeField, BoxGroup("Bonus")] private Color _bonusValueColor;
        [SerializeField, BoxGroup("ForAll")] private GameObject _appliesForAll;
        [SerializeField, BoxGroup("Enhance")] private GameObject _enhanceCondition;
        [SerializeField, BoxGroup("Enhance")] private GameObject _evolutionCondition;
        [SerializeField, BoxGroup("Enhance")] private TextMeshProUGUI _evolutionConditionText;
        [SerializeField, BoxGroup("Enhance")] private GameObject _levelUpCondition;
        [SerializeField, BoxGroup("Enhance")] private TextMeshProUGUI _levelUpConditionText;

        public bool IsEnabled => gameObject.activeSelf;

        public void Enable(ICharacterBonus characterBonus)
        {
            gameObject.SetActive(true);
            _container.anchoredPosition = Vector2.zero;
            _screenFitter.Fit(_container);

            _appliesForAll.SetActive(characterBonus.IsAlwaysApply());
            
            string enhanceCondition = characterBonus.GetEnhanceCondition();
            if (string.IsNullOrEmpty(enhanceCondition)) 
                _enhanceCondition.SetActive(false);
            else
            {
                _enhanceCondition.SetActive(true);
                switch (characterBonus.ConditionType)
                {
                    case BonusConditionType.Level:
                        _levelUpConditionText.SetText(enhanceCondition);
                        _levelUpCondition.SetActive(true);
                        _evolutionCondition.SetActive(false);
                        break;
                    case BonusConditionType.Evolution:
                        _evolutionConditionText.SetText(enhanceCondition);
                        _levelUpCondition.SetActive(false);
                        _evolutionCondition.SetActive(true);
                        break;
                }   
            }

            switch (characterBonus.ConditionType)
            {
                case BonusConditionType.Level:
                    _blockVisual.SetActive(!characterBonus.IsUnlocked);

                    string levelBonusValue = $"<color={_bonusValueColor.ToHexString()}>{characterBonus.GetStatValue(characterBonus.IsUnlocked ? characterBonus.Level : 1)}</color>";
                    string levelBonus = $"{characterBonus.GetStatName()}\n{levelBonusValue}";
                    _bonusText.SetText(levelBonus);
                    break;
                case BonusConditionType.Evolution:
                    _blockVisual.SetActive(false);
                    string evolutionBonusValue = $"<color={_bonusValueColor.ToHexString()}>{characterBonus.GetStatValue(characterBonus.Level)}";
                    string evolutionBonus = $"{characterBonus.GetStatName()}\n{evolutionBonusValue}";
                    _bonusText.SetText(evolutionBonus);
                    break;
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}