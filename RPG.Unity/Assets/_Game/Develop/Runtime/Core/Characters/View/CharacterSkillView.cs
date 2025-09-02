using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CharacterSkillView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private List<GameObject> _blockObjects;
        [SerializeField] private TextMeshProUGUI _unlockConditionText;

        [Inject] private ITranslator _translator;
        private Character _character;
        
        public void Setup(Character character)
        {
            if(_character != null)
                ClearCharacter();
            
            _character = character;
            _character.OnLevelUp += OnCharacterLevelUp;
            
            _image.sprite = _character.SkillImage;
            _name.SetText(_translator.Translate(_character.Skill.NameId));
            _description.SetText(_character.Skill.GetDescription());

            RedrawBlock();
        }

        private void ClearCharacter()
        {
            _character.OnLevelUp -= OnCharacterLevelUp;
            _character = null;
        }

        private void OnCharacterLevelUp(Character character) => 
            RedrawBlock();

        private void RedrawBlock()
        {
            if (_character.IsSkillUnlocked)
            {
                foreach (var blockObject in _blockObjects) 
                    blockObject.SetActive(false);
            }
            else
            {
                foreach (var blockObject in _blockObjects) 
                    blockObject.SetActive(true);
                _unlockConditionText.SetText($"{TranslationConst.LevelPrefixCaps} {_character.SkillUnlockLevel}");
            }
        }
    }
}