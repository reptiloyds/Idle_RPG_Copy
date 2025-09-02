using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CharacterPresenter : MonoBehaviour
    {
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Image _blockImage;
        [SerializeField, Required] private Image _rarityImage;
        [SerializeField, Required] private TextMeshProUGUI _nameText;
        [SerializeField, Required] private GameObject _equippedObject;
        [SerializeField, Required] private EvolutionLevelProgressionView _evolutionLevelProgressionView;
        [SerializeField, Required] private GameObject _unownedObject;
        [SerializeField, Required] private CharacterBonusPresenter _characterBonusPresenter;
        [SerializeField, Required] private CharacterSkillView _characterSkillView;

        [Inject] private CharacterService _characterService;
        [Inject] private ITranslator _translator;
        
        private Character _character;

        private void Awake() => 
            _characterService.OnAnySwitched += OnCharacterSwitched;

        private void OnDestroy() => 
            _characterService.OnAnySwitched -= OnCharacterSwitched;

        public void Setup(Character character)
        {
            Clear();
            _character = character;
            _character.OnEvolve += OnEvolve;
            _character.OnOwned += OnOwned;
            _character.OnExperienceChanged += OnExperienceChanged;
            _character.OnLevelUp += OnLevelUp;

            RedrawAll();
        }

        private void Clear()
        {
            if (_character == null) return;
            
            _character.OnEvolve -= OnEvolve;
            _character.OnOwned -= OnOwned;
            _character.OnExperienceChanged -= OnExperienceChanged;
            _character.OnLevelUp -= OnLevelUp;
            _character = null;
        }

        private void OnLevelUp(Character character) => 
            RedrawLevelProgression();

        private void OnExperienceChanged(Character character) => 
            RedrawLevelProgression();

        private void OnOwned(Character character) => 
            RedrawAll();

        private void OnEvolve(Character character) => 
            RedrawAll();

        private void OnCharacterSwitched() => 
            RedrawEquipped();

        private void RedrawAll()
        {
            _image.sprite = _character.Sprite;
            _rarityImage.sprite = _character.RarityImage;
            _nameText.SetText(_character.FormattedName);
            
            if (_character.IsOwned)
            {
                _blockImage.gameObject.SetActive(false);
                _evolutionLevelProgressionView.gameObject.SetActive(true);
                _unownedObject.SetActive(false);
                RedrawEvolution();
                RedrawLevelProgression();
            }
            else
            {
                _blockImage.sprite = _character.Sprite;
                _blockImage.gameObject.SetActive(true);
                _evolutionLevelProgressionView.gameObject.SetActive(false);
                _unownedObject.SetActive(true);
            }
            
            RedrawEquipped();
            RedrawBonuses();
            RedrawSkill();
        }
        
        private void RedrawEvolution() =>
            _evolutionLevelProgressionView.RedrawEvolution(_character.Evolution);

        private void RedrawLevelProgression()
        {
            _evolutionLevelProgressionView.Redraw(_character.Level, _character.IsMaxEnhanced || _character.IsEvolutionReady(),
                _character.Experience, _character.TargetExperience);
        }

        private void RedrawEquipped() => 
            _equippedObject.SetActive(_character.IsEquipped);

        private void RedrawSkill() => 
            _characterSkillView.Setup(_character);

        private void RedrawBonuses() => 
            _characterBonusPresenter.Setup(_character.GetBonuses());
    }
}