using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Tag;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model
{
    public class StuffItem : Item
    {
        private readonly StuffItemRow _stuffConfig;
        private readonly BaseValueFormula _equippedValueFormula;
        private StatModifier _equippedModifier;
        private readonly List<UnitStat> _equippedTargets = new();

        public override ItemType Type => ItemType.Stuff;
        
        public StuffSlotType SlotType { get; private set; }

        public UnitStatType EquippedEffectType => _stuffConfig.EquippedEffectType;
        public StatModifier EquippedModifier => _equippedModifier;


        public StuffItem(ITranslator translator, StuffItemRow stuffConfig, ItemData data, StuffSlotType slotType, ItemConfiguration configuration, Sprite sprite, BaseValueFormula ownedFormula, BaseValueFormula equippedFormula) : 
            base(translator, stuffConfig, data, configuration, sprite, ownedFormula)
        {
            SlotType = slotType;
            _stuffConfig = stuffConfig;
            _equippedValueFormula = equippedFormula ?? stuffConfig.EquippedFormulaType.CreateFormula(stuffConfig.EquippedFormulaJSON);

            CreateEquippedModifier();
        }

        protected override void LevelUp()
        {
            var oldMod = _equippedModifier;
            CreateEquippedModifier(1);
            foreach (var target in _equippedTargets) 
                target.ReplaceModifier(oldMod, _equippedModifier);
            
            base.LevelUp();
        }

        public void AddEquippedTarget(UnitStat stat)
        {
            _equippedTargets.Add(stat);
            stat.AddModifier(_equippedModifier);
        }

        public void RemoveEquippedTarget(UnitStat stat)
        {
            _equippedTargets.Remove(stat);
            stat.RemoveModifier(_equippedModifier);
        }

        private void CreateEquippedModifier(int levelDelta = 0) => 
            _equippedModifier = new StatModifier(_equippedValueFormula.CalculateBigDouble(Level + levelDelta), _stuffConfig.EquippedEffectModType, this, GroupOrder.Stuff);
    }
}