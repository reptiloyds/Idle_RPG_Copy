using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model
{
    public class SkillInventory : BaseInventory<SkillItem>, IDisposable
    {
        [Inject] private ContentService _contentService;
        [Inject] private SkillDataProvider _dataProvider;
        
        private SkillItemSheet _configs;
        private ManualFormulaSheet<SkillItemSheet, ItemRarityType> _manualOwnedValues;
        
        private SkillDataContainer _data;
        private readonly List<SkillSlot> _slots = new();

        public IReadOnlyList<SkillSlot> Slots => _slots;
        protected override ItemType Type => ItemType.Skill;
        public event Action<SkillItem> OnEquipFailed;
        public event Action<SkillSlot, SkillItem> OnEquipped;
        public event Action<SkillSlot, SkillItem> OnRemoved;

        [Preserve]
        public SkillInventory() { }

        public override void Initialize()
        {
            _data = _dataProvider.GetData();
            _configs = Balance.Get<SkillItemSheet>();
            _manualOwnedValues = Balance.Get<ManualFormulaSheet<SkillItemSheet, ItemRarityType>>();
            base.Initialize();
            
            CreateSlots();
        }

        protected override SkillItem CreateItem(ItemData data)
        {
            if (!_configs.Contains(data.Id))
            {
                Logger.LogError($"There is no skill with id {data.Id} in the companion sheet.");
                return null;
            }
            var config = _configs[data.Id];
            BaseValueFormula formula = null;
            if (config.OwnedFormulaType == FormulaType.CustomSheet) 
                formula = _manualOwnedValues.GetValueFormula(config.RarityType);
            var sprite = SpriteProvider.GetSprite(Asset.SkillAtlas, config.ImageName);
            var inactiveSprite = SpriteProvider.GetSprite(Asset.SkillAtlas, $"{config.ImageName}_inactive");
            return new SkillItem(Translator, config, data, Configuration, formula, sprite, inactiveSprite);
        }

        private void CreateSlots()
        {
            foreach (var data in _data.List)
            {
                var slotModel = new SkillSlot(data, _contentService.GetSkill(data.SlotId));
                var item = GetItem(data.ItemId);
                if(item != null)
                    slotModel.Equip(item);
                _slots.Add(slotModel);
            }
        }

        public bool Equip(SkillItem item)
        {
            foreach (var slotModel in _slots)
            {
                if(slotModel.BaseItem != null) continue;
                if(!slotModel.IsUnlocked) continue;
                slotModel.Equip(item);
                OnEquipped?.Invoke(slotModel, item);
                return true;
            }
            
            OnEquipFailed?.Invoke(item);
            return false;
        }

        public bool Remove(SkillItem item)
        {
            foreach (var slotModel in _slots)
            {
                if(slotModel.BaseItem != item) continue;
                slotModel.Remove();
                OnRemoved?.Invoke(slotModel, item);
                return true;
            }
            
            return false;
        }

        void IDisposable.Dispose()
        {
            foreach (var slotModel in _slots) 
                slotModel.Dispose();
        }
    }
}