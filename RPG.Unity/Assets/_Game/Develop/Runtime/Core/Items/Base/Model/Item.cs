using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.Model
{
    public abstract class Item
    { 
        private readonly ItemData _data;
        private readonly BaseValueFormula _upgradeValueFormula;
        private int _targetAmount;
        private readonly int _maxLevel;
        
        private readonly BaseValueFormula _ownedValueFormula;
        private StatModifier _ownedModifier;
        private readonly List<UnitStat> _ownedTargets = new();

        private readonly ItemRow _config;

        public bool IsUnlocked => _data.IsUnlocked;
        public int Level => _data.Level;
        public int Amount => _data.Amount;
        public int TargetAmount => _targetAmount;

        public abstract ItemType Type { get; }

        [ShowInInspector]
        public string Id => _data.Id;
        public Sprite Sprite { get; }
        public UnitStatType OwnedEffectType => _config.OwnedEffectType;
        public StatModifier OwnedModifier => _ownedModifier;
        public ItemRarityType Rarity => _config.RarityType;
        public Color RarityColor { get; }
        public bool IsEquipped { get; private set; }
        public bool CanEnhance => _data.Amount >= _targetAmount && _data.Level < _maxLevel;
        public bool IsLevelMax => _data.Level >= _maxLevel;
        public string LocalizedRarity { get; }
        public string LocalizedName{ get; }

        public event Action<Item> OnEquip;
        public event Action<Item> OnTakeOff;
        public event Action<Item> OnAmountChanged; 
        public event Action<Item> OnUnlock;
        public event Action<Item> OnLevelUp;

        protected Item(ITranslator translator, ItemRow config, ItemData data, ItemConfiguration configuration,
            Sprite sprite, BaseValueFormula ownedValueFormula = null)
        {
            _config = config;
            LocalizedRarity = translator.Translate(_config.RarityType.ToString());
            LocalizedName = translator.Translate(_config.LocalizationToken);
            
            _data = data;
            Sprite = sprite;
            configuration.GetColor(Rarity, out var rarityColor);
            RarityColor = rarityColor;
            _maxLevel = config.MaxLevel;

            _ownedValueFormula = ownedValueFormula ?? config.OwnedFormulaType.CreateFormula(config.OwnedFormulaJSON);
            _upgradeValueFormula = config.UpgradeFormulaType.CreateFormula(config.UpgradeFormulaJSON);
            UpdateTargetAmount();
            
            CreateOwnedModifier();
        }

        public bool CanAdd(int amount)
        {
            if(amount <= 0) return false;

            var tempAmount = _data.Amount;
            var tempTarget = _targetAmount;
            var tempLevel = _data.Level;
            while (tempLevel < _maxLevel)
            {
                if (tempAmount >= tempTarget)
                {
                    TempLevelUp();
                    continue;
                }
                
                amount -= tempTarget - tempAmount;
                if (amount <= 0) return true;
                TempLevelUp();
            }

            void TempLevelUp()
            {
                tempLevel++;
                tempAmount -= tempTarget;
                tempTarget = (int)_upgradeValueFormula.CalculateBigDouble(tempLevel).ToDouble();
            }

            return false;
        }

        public void AddAmount(int amount)
        {
            if(amount <= 0) return;

            if (!IsUnlocked)
            {
                if (amount > 1) _data.Amount = amount - 1;
                Unlock();
            }
            else
                _data.Amount += amount;
            
            OnAmountChanged?.Invoke(this);
        }

        public void AddOwnedTarget(UnitStat stat)
        {
            _ownedTargets.Add(stat);
            if(_data.IsUnlocked)
                stat.AddModifier(_ownedModifier);
        }
        
        public void Enhance()
        {
            LevelUp();
            if(CanEnhance)
                Enhance();
            else
                OnLevelUp?.Invoke(this);
        }

        public void Equip()
        {
            IsEquipped = true;
            OnEquip?.Invoke(this);
        }

        public void TakeOff()
        {
            IsEquipped = false;
            OnTakeOff?.Invoke(this);
        }

        private void Unlock()
        {
            _data.IsUnlocked = true;
            foreach (var ownedTarget in _ownedTargets)
                ownedTarget.AddModifier(_ownedModifier);
            OnUnlock?.Invoke(this);
        }

        protected virtual void LevelUp()
        {
            _data.Level++;
            _data.Amount -= _targetAmount;
            UpdateTargetAmount();
            UpdateOwnedModifiers();
        }

        private void UpdateOwnedModifiers()
        {
            var oldMod = _ownedModifier;
            CreateOwnedModifier();
            foreach (var ownedTarget in _ownedTargets)
                ownedTarget.ReplaceModifier(oldMod, _ownedModifier);  
        }

        private void CreateOwnedModifier() => 
            _ownedModifier = new StatModifier(_ownedValueFormula.CalculateBigDouble(Level), _config.OwnedEffectModType, this, _config.OwnedEffectOrder);

        private void UpdateTargetAmount() => 
            _targetAmount = (int)_upgradeValueFormula.CalculateBigDouble(Level).ToDouble();
    }
}