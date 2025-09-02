using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Sheet;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model
{
    public class CompanionInventory : BaseInventory<CompanionItem>, IDisposable
    {
        [Inject] private ContentService _contentService;
        [Inject] private CompanionDataProvider _companionDataProvider;

        private CompanionItemSheet _configs;
        private ManualFormulaSheet<CompanionItemSheet, ItemRarityType> _manualOwnedValues;
        
        private CompanionDataContainer _data;
        private readonly List<CompanionSlot> _slots = new();
        
        protected override ItemType Type => ItemType.Companion;

        public IReadOnlyList<CompanionSlot> Slots => _slots;
        public event Action<CompanionItem> OnEquipFailed;
        public event Action<CompanionSlot, CompanionItem> OnEquipped;
        public event Action<CompanionSlot, CompanionItem> OnRemoved;

        [Preserve]
        public CompanionInventory() { }

        public override void Initialize()
        {
            _data = _companionDataProvider.GetData();
            _configs = Balance.Get<CompanionItemSheet>();
            _manualOwnedValues = Balance.Get<ManualFormulaSheet<CompanionItemSheet, ItemRarityType>>();
            base.Initialize();
            
            CreateSlots();
        }

        protected override CompanionItem CreateItem(ItemData data)
        {
            if (!_configs.Contains(data.Id))
            {
                Logger.LogError($"There is no companion with id {data.Id} in the companion sheet.");
                return null;
            }
            var config = _configs[data.Id];
            BaseValueFormula formula = null;
            if (config.OwnedFormulaType == FormulaType.CustomSheet) 
                formula = _manualOwnedValues.GetValueFormula(config.RarityType);
            return new CompanionItem(Translator, config, data, Configuration, SpriteProvider.GetSprite(Asset.CompanionAtlas, config.ImageName), formula);
        }

        private void CreateSlots()
        {
            foreach (var data in _data.List)
            {
                var slotModel = new CompanionSlot(data, _contentService.GetCompanion(data.SlotId));
                var item = GetItem(data.ItemId);
                if(item != null)
                    slotModel.Equip(item);
                _slots.Add(slotModel);
            }
        }

        public bool Equip(string itemId)
        {
            var item = GetItem(itemId);
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

        public bool Remove(string itemId)
        {
            var item = GetItem(itemId);
            foreach (var slotModel in _slots)
            {
                if(slotModel.BaseItem != item) continue;
                slotModel.Remove();
                OnRemoved?.Invoke(slotModel, item);
                return true;
            }
            
            return false;
        }

        public void Dispose()
        {
            foreach (var slotModel in _slots) 
                slotModel.Dispose();
        }
    }
}