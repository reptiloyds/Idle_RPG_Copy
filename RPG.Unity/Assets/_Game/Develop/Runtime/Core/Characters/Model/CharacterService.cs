using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus.Conditions;
using PleasantlyGames.RPG.Runtime.Core.Characters.Save;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.Characters.Type;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Model;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Model
{
    public class CharacterService : IDisposable
    {
        [Inject] private BalanceContainer _balance;
        [Inject] private GlobalStatProvider _globalStatProvider;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private ITranslator _translator;
        [Inject] private ResourceService _resourceService;
        [Inject] private CharacterSkillService _skillService;
        [Inject] private BranchService _branchService;
        [Inject] private ISpriteProvider _spriteProvider;

        private GlobalStat _experienceStat;
        private CharacterDataContainer _data;
        private CharacterSheet _sheet;
        private readonly List<Character> _characters = new();
        private readonly Dictionary<string, List<Character>> _branchCharacters = new();

        public IReadOnlyList<Character> Characters => _characters;
        public event Action<Character, ICharacterBonus> OnBonusUp;
        public event Action<Character> OnEvolved;
        public event Action<Character> OnLevelUp;
        public event Action OnAnySwitched;
        public event Action<Character, Character> OnSwitched;

        [Preserve]
        public CharacterService()
        {
        }

        public void Setup(CharacterDataContainer data) =>
            _data = data;

        public void Initialize()
        {
            _sheet = _balance.Get<CharacterSheet>();
            _experienceStat = _globalStatProvider.GetStat(GlobalStatType.CharacterExperience);

            CreateCharacters();
            InitializeBonuses();
            CheckOwnership();
            CheckEquipment();

            _branchService.BranchUnlock += OnBranchUnlock;
        }

        public void Dispose()
        {
            foreach (var character in _characters)
            {
                character.OnBonusLevelUp -= OnCharacterBonusUp;
                character.OnLevelUp -= OnCharacterLevelUp;
            }

            _branchService.BranchUnlock -= OnBranchUnlock;
        }

        public Character GetCharacter(string characterId)
        {
            foreach (var character in _characters)
                if (character.Id == characterId)
                    return character;

            return null;
        }

        public IReadOnlyList<Character> GetCharactersForBranch(string branchId)
        {
            if (_branchCharacters.TryGetValue(branchId, out var list)) return list;

            list = new List<Character>();
            foreach (var character in _characters)
                if (character.BranchIds.Contains(branchId))
                    list.Add(character);

            _branchCharacters.Add(branchId, list);

            return list;
        }

        public void Own(Character character) =>
            character.Own();

        public void AddExperience(Character character)
        {
            var amount = (int)_experienceStat.Value.ToDouble();
            character.AddExperience(amount);
        }

        public void Evolve(Character character)
        {
            var prices = character.GetEvolutionPrice();
            foreach (var price in prices)
                _resourceService.SpendResource(price.Type, price.GetValue());
            character.Evolve();
            OnEvolved?.Invoke(character);
        }

        public void SwitchCharacter(string characterId)
        {
            var selectedBranch = _branchService.GetSelectedBranch();
            if (string.Equals(selectedBranch.CharacterId, characterId)) return;
            var newCharacter = GetCharacter(characterId);
            if (newCharacter == null) return;

            var previousCharacter = GetCharacter(selectedBranch.CharacterId);
            previousCharacter.TakeOff();
            newCharacter.Equip();
            _branchService.SetCharacterToSelectedBranch(characterId);

            OnAnySwitched?.Invoke();
            OnSwitched?.Invoke(previousCharacter, newCharacter);
        }

        private void CreateCharacters()
        {
            var sheet = _balance.Get<UnitsSheet>();
            foreach (var data in _data.List)
            {
                if (!_sheet.Contains(data.Id)) continue;

                var config = _sheet[data.Id];
                var bonuses = CreateBonuses(config);
                var character = new Character(data, config, _skillService, _translator, _spriteProvider, bonuses,
                    sheet);

                character.OnBonusLevelUp += OnCharacterBonusUp;
                character.OnLevelUp += OnCharacterLevelUp;
                _characters.Add(character);
            }
        }

        private void InitializeBonuses()
        {
            foreach (var character in _characters)
            foreach (var bonus in character.GetBonuses())
                bonus.Initialize(character);
        }

        private void CheckOwnership()
        {
            foreach (var character in _characters)
            {
                if (character.IsOwned || character.OwnType != CharacterOwnType.Free) return;
                foreach (var branchId in character.BranchIds)
                {
                    var branch = _branchService.GetBranch(branchId);
                    if (!branch.IsUnlocked) continue;
                    character.Own();
                }
            }
        }

        private void CheckEquipment()
        {
            foreach (var branch in _branchService.Branches)
            {
                if (!branch.IsUnlocked) continue;
                GetCharacter(branch.CharacterId)?.Equip();
            }
        }

        private void OnBranchUnlock(Branch branch)
        {
            // foreach (var character in _characters)
            // {
            //     if (character.IsOwned || character.OwnType != CharacterOwnType.Free) continue;
            //     if (!character.BranchIds.Contains(branch.Id)) continue;
            //     character.Own();
            // }

            var character = GetCharacter(branch.DefaultCharacterId);
            if (!character.IsOwned)
                character.Own();
            character.Equip();
        }

        private void OnCharacterBonusUp(Character character, ICharacterBonus bonus) =>
            OnBonusUp?.Invoke(character, bonus);

        private List<ICharacterBonus> CreateBonuses(CharacterRow config)
        {
            var bonuses = new List<ICharacterBonus>();
            var atlasName = Asset.MainAtlas;
            foreach (var bonusConfig in config)
            {
                IBonusCondition condition = null;
                switch (bonusConfig.BonusConditionType)
                {
                    case BonusConditionType.Level:
                        condition = new LevelCondition(_translator, bonusConfig.BonusConditionJSON);
                        break;
                    case BonusConditionType.Evolution:
                        condition = new EvolutionCondition(_translator, bonusConfig.BonusConditionJSON);
                        break;
                }

                var sprite = _spriteProvider.GetSprite(atlasName, bonusConfig.ImageName);
                switch (bonusConfig.StatType)
                {
                    case StatType.Unit:
                        var unitStatBonus = new UnitCharacterBonus(bonusConfig, condition, sprite);
                        _objectResolver.Inject(unitStatBonus);
                        bonuses.Add(unitStatBonus);
                        break;
                    case StatType.Global:
                        var globalStatBonus = new GlobalCharacterBonus(bonusConfig, condition, sprite);
                        _objectResolver.Inject(globalStatBonus);
                        bonuses.Add(globalStatBonus);
                        break;
                }
            }

            return bonuses;
        }

        private void OnCharacterLevelUp(Character character) =>
            OnLevelUp?.Invoke(character);
    }
}