using System;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CharacterBonusView : MonoBehaviour
    {
        [SerializeField, Required] private Image _background;
        [SerializeField, Required] private Sprite _blockedBackground;
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Image _blockImage;
        
        [SerializeField, Required, BoxGroup("Level")] private Sprite _defaultBackground;
        [SerializeField, Required, BoxGroup("Level")] private TextMeshProUGUI _levelConditionText;
        [SerializeField, Required, BoxGroup("Level")] private GameObject _bonusLevelObject; 
        [SerializeField, Required, BoxGroup("Level")] private TextMeshProUGUI _bonusLevelText; 
        
        [SerializeField, Required, BoxGroup("Evolution")] private Sprite _evolutionBackground;
        [SerializeField, Required, BoxGroup("Evolution")] private GameObject _evolutionObject;
        [SerializeField, Required, BoxGroup("Evolution")] private TextMeshProUGUI _evolutionText;
        
        [SerializeField, Required] private BaseButton _button;
        [SerializeField, Required] private CharacterBonusHintView _hintView;

        private ICharacterBonus _characterBonus;
        public event Action<CharacterBonusView> OnClick;

        private void Awake()
        {
            _button.OnClick += OnButtonClick;
            if(_hintView.IsEnabled)
                HideHint();
        }

        private void OnDestroy() => 
            _button.OnClick -= OnButtonClick;

        private void OnButtonClick() => 
            OnClick?.Invoke(this);

        public void Setup(ICharacterBonus characterBonus)
        {
            if(_characterBonus != null)
                ClearBonus();
            
            _characterBonus = characterBonus;
            _characterBonus.OnLevelUp += OnBonusLevelUp;
            _image.sprite = _characterBonus.Sprite;
            _blockImage.sprite = _characterBonus.Sprite;
            
            Redraw();
        }

        private void Redraw()
        {
            Sprite backgroundSprite;
            var conditionIsEvolution = _characterBonus.ConditionType == BonusConditionType.Evolution;
            if(conditionIsEvolution)
                _evolutionText.SetText(_characterBonus.GetMetConditions().ToString());
            _evolutionObject.gameObject.SetActive(conditionIsEvolution);

            if (conditionIsEvolution)
            {
                _bonusLevelObject.SetActive(false);
                _levelConditionText.gameObject.SetActive(false);
            }
            else
            {
                if (_characterBonus.IsUnlocked)
                {
                    _levelConditionText.gameObject.SetActive(false);
                    _bonusLevelObject.SetActive(true);
                    _bonusLevelText.SetText(_characterBonus.Level.ToString());   
                }
                else
                {
                    _bonusLevelObject.SetActive(false);
                    _levelConditionText.gameObject.SetActive(true);
                    _levelConditionText.SetText(_characterBonus.GetUnlockCondition());   
                }
            }
            
            if (_characterBonus.IsUnlocked)
            {
                backgroundSprite = conditionIsEvolution ? _evolutionBackground : _defaultBackground;
                _blockImage.gameObject.SetActive(false);
            }
            else
            {
                backgroundSprite = _blockedBackground;
                _blockImage.gameObject.SetActive(true);
            }
            _background.sprite = backgroundSprite;
        }

        private void ClearBonus()
        {
            _characterBonus.OnLevelUp -= OnBonusLevelUp;
            _characterBonus = null;
        }

        private void OnBonusLevelUp(ICharacterBonus characterBonus) => Redraw();

        public void ShowHint() => _hintView.Enable(_characterBonus);

        public void HideHint() => _hintView.Disable();
    }
}