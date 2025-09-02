using System;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CharacterCard : MonoBehaviour
    {
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Image _blockImage;
        [SerializeField, Required] private Image _rarityImage;
        [SerializeField, Required] private BaseButton _button;
        [SerializeField, Required] private TextMeshProUGUI _nameText;
        [SerializeField, Required] private GameObject _evolutionObject;
        [SerializeField, Required] private TextMeshProUGUI _evolutionText;
        [SerializeField, Required] private TextMeshProUGUI _levelEvolutionText;
        [SerializeField, Required] private TextMeshProUGUI _levelText;
        [SerializeField, Required] private GameObject _equippedObject;
        [SerializeField, Required] private GameObject _selectedObject;
        [SerializeField, Required] private Image _backgroundImage;
        [SerializeField, Required] private Color _unlockColor;
        [SerializeField, Required] private Color _lockedColor;

        [ShowInInspector, HideInEditorMode, ReadOnly]
        private Character _character;

        [Inject] private ITranslator _translator;

        public event Action<CharacterCard> OnClick;
        public Character Character => _character;

        private void Awake() => 
            _button.OnClick += OnButtonClick;

        private void OnDestroy() => 
            _button.OnClick -= OnButtonClick;

        private void OnButtonClick() => 
            OnClick?.Invoke(this);

        public void SetupButtonId(string id) => 
            _button.ChangeButtonId(id);

        public void Setup(Character character)
        {
            _character = character;
            _character.OnOwned += OnOwned;
            _character.OnLevelUp += OnLevelUp;
            _character.OnEvolve += OnEvolve;
            Redraw();
        }

        public void Clear()
        {
            if (_character != null)
            {
                _character.OnOwned -= OnOwned;
                _character.OnLevelUp -= OnLevelUp;
                _character.OnEvolve -= OnEvolve;
                _character = null;   
            }
        }

        private void OnEvolve(Character character) => 
            Redraw();

        private void OnOwned(Character character) => 
            Redraw();

        private void OnLevelUp(Character character) => 
            Redraw();

        private void Redraw()
        {
            _nameText.SetText(_character.FormattedName);
            _image.sprite = _character.Sprite;
            
            if (_character.IsOwned)
            {
                _blockImage.gameObject.SetActive(false);
                _backgroundImage.color = _unlockColor;
                
                if (_character.Evolution > 0)
                {
                    _levelEvolutionText.SetText(_character.Level.ToString());
                    _evolutionText.SetText(_character.Evolution.ToString());
                    _evolutionObject.SetActive(true);
                    _levelText.gameObject.SetActive(false);
                }
                else
                {
                    _levelText.SetText($"{TranslationConst.LevelPrefixCaps} {_character.Level}");
                    _evolutionObject.SetActive(false);
                    _levelText.gameObject.SetActive(true);
                }
            }
            else
            {
                _levelText.SetText(_translator.Translate(TranslationConst.Unowned));
                _blockImage.sprite = _character.Sprite;
                _blockImage.gameObject.SetActive(true);
                _backgroundImage.color = _lockedColor;
                _levelText.gameObject.SetActive(true);
                _evolutionObject.SetActive(false);
            }

            _rarityImage.sprite = _character.RarityImage;
        }

        public void Equipped() => 
            _equippedObject.SetActive(true);

        public void Unequipped() => 
            _equippedObject.SetActive(false);

        public void Selected() => 
            _selectedObject.gameObject.SetActive(true);

        public void Unselected() => 
            _selectedObject.gameObject.SetActive(false);
    }
}