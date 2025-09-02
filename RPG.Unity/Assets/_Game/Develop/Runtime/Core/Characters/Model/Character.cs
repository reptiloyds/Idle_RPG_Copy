using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus;
using PleasantlyGames.RPG.Runtime.Core.Characters.Save;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.Characters.Type;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Deal.Model;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Model
{
    public class Character
    {
        private readonly CharacterSkillService _skillService;
        private readonly ISpriteProvider _spriteProvider;
        private readonly List<ICharacterBonus> _bonuses;
        private readonly UnitsSheet.Row _unitSheet;

        private readonly CharacterData _data;
        private readonly CharacterRow _config;
        private readonly BaseValueFormula _levelFormula;

        private int _targetExp;
        private EvolutionData _evolution;
        private int _evolutionLevel = -1;
        private readonly int _maxEvolutionId;

        public bool IsOwned => _data.IsOwned;
        public int Level => _data.Level;
        public int Experience => _data.Experience;
        public int Evolution => _data.Evolution;
        public int MaxEvolution => _maxEvolutionId;
        public bool IsMaxEnhanced => _data.Level >= _config.MaxLevel && _data.Evolution == _maxEvolutionId;

        public string Id => _config.Id;
        public string FormattedName { get; }
        public string UnitId => _config.UnitId;
        public Sprite Sprite { get; private set; }
        public CharacterOwnType OwnType => _config.OwnType;
        public CharacterRarityType RarityType => _config.Rarity;
        public List<ResourcePriceStruct> PurchasePrice { get; }
        public int TargetExperience => _targetExp;
        public Sprite SkillImage { get; }
        public Sprite RarityImage { get; }
        public int SkillUnlockLevel => _config.SkillLevelRequest;
        public bool IsSkillUnlocked => Level >= _config.SkillLevelRequest;
        public Skill.Model.Skill Skill { get; }
        public bool IsEquipped { get; private set; }
        public IReadOnlyList<string> BranchIds => _config.Branches;
        public Sprite MainImage { get; }

        public event Action<Character> OnLevelUp;
        public event Action<Character> OnEvolve;
        public event Action<Character> OnOwned;
        public event Action<Character> OnExperienceChanged;
        public event Action<Character, ICharacterBonus> OnBonusLevelUp;

        public Character(CharacterData data, CharacterRow config, CharacterSkillService skillService,
            ITranslator translator, ISpriteProvider spriteProvider, List<ICharacterBonus> bonuses,
            UnitsSheet unitsSheet)
        {
            _data = data;
            _config = config;
            _bonuses = bonuses;
            _skillService = skillService;
            _spriteProvider = spriteProvider;
            _unitSheet = unitsSheet[UnitId];

            SkillImage = _spriteProvider.GetSprite(Asset.SkillAtlas, config.SkillImage);
            RarityImage = _spriteProvider.GetSprite(Asset.MainAtlas, $"CharRarity_{RarityType}");

            foreach (var bonus in _bonuses)
                bonus.OnLevelUp += OnBonusUp;

            Skill = _skillService.GetCharacterSkill(config.SkillId);

            FormattedName = translator.Translate(config.LocalizationToken);

            _maxEvolutionId = _config.Evolutions.Count - 1;

            PurchasePrice = config.OwnType == CharacterOwnType.Resource
                ? JsonConvertLog.DeserializeObject<List<ResourcePriceStruct>>(config.PurchaseData)
                : null;

            _levelFormula = _config.LevelFormula.CreateFormula(_config.LevelFormulaJSON);
            UpdateTargetExp();
            UpdateEvolution();
            CalculateEvolutionLevel();
            
            MainImage = _spriteProvider.GetSprite(_unitSheet.MainImage);
        }

        public void Own()
        {
            _data.IsOwned = true;
            CalculateEvolutionLevel();
            OnOwned?.Invoke(this);
        }

        public void AddExperience(int amount)
        {
            _data.Experience += amount;
            OnExperienceChanged?.Invoke(this);
            if (_data.Experience >= _targetExp)
                LevelUp();
        }
        
        public bool IsEvolutionReady()
        {
            if (_evolutionLevel <= 0) return false;
            return _data.Level >= _evolutionLevel;
        }

        public List<ResourcePriceStruct> GetEvolutionPrice()
        {
            if (_evolutionLevel <= 0) return null;
            return _config.Evolutions[_config.Evolutions.IndexOf(_evolution) + 1].Price;
        }

        public int GetCurrentLevelCap()
        {
            if (_evolutionLevel < 0)
                return _config.MaxLevel;
            return _evolutionLevel;
        }

        public int GetNextLevelCap()
        {
            var nextEvolutionIndex = _config.Evolutions.IndexOf(_evolution) + 2;
            if (_config.Evolutions.Count <= nextEvolutionIndex)
                return _config.MaxLevel;
            return _config.Evolutions[nextEvolutionIndex].Level;
        }

        public List<ICharacterBonus> GetBonuses() =>
            _bonuses;
        
        public Sprite GetEvolutionSprite(int evolutionLevel)
        {
            var images = _unitSheet.Images;
            if (images.Count == 0)
                return null;
            var imageIndex = Math.Min(images.Count - 1, evolutionLevel);
            return _spriteProvider.GetSprite(Asset.UnitAtlas, images[imageIndex]);
        }

        public void TakeOff()
        {
            IsEquipped = false;
            foreach (var bonus in _bonuses)
                bonus.OnTakeOff();

            if (IsSkillUnlocked)
                _skillService.DisableCharacterSkill(Skill);
        }

        public void Equip()
        {
            IsEquipped = true;
            foreach (var bonus in _bonuses)
                bonus.OnEquip();

            if (IsSkillUnlocked)
                _skillService.EnableCharacterSkill(Skill);
        }
        
        public void Evolve()
        {
            _data.Evolution++;
            UpdateEvolution();
            CalculateEvolutionLevel();
            OnEvolve?.Invoke(this);
        }

        private void OnBonusUp(ICharacterBonus bonus) =>
            OnBonusLevelUp?.Invoke(this, bonus);

        private void LevelUp()
        {
            _data.Level++;
            _data.Experience -= _targetExp;
            UpdateTargetExp();

            if (IsEquipped && SkillUnlockLevel == _data.Level)
                _skillService.EnableCharacterSkill(Skill);

            OnLevelUp?.Invoke(this);
        }

        private void CalculateEvolutionLevel()
        {
            if (_data.IsOwned)
            {
                var evolutionIndex = _config.Evolutions.IndexOf(_evolution) + 1;
                if (_config.Evolutions.Count <= evolutionIndex)
                    _evolutionLevel = -1;
                else
                    _evolutionLevel = _config.Evolutions[evolutionIndex].Level;
            }
            else
                _evolutionLevel = -1;
        }

        private void UpdateTargetExp() =>
            _targetExp = (int)_levelFormula.CalculateBigDouble(_data.Level).ToDouble();

        private void UpdateEvolution()
        {
            var evolution = _config.Evolutions[_data.Evolution];
            if (_evolution == evolution) return;
            _evolution = evolution;
            Sprite = GetEvolutionSprite(_data.Evolution);
        }
    }
}