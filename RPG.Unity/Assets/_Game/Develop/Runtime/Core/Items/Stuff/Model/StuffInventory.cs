using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Branches.Sheet;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Tag;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model
{
    public class StuffInventory : BaseInventory<StuffItem>, IDisposable
    {
        [Inject] private ContentService _contentService;
        [Inject] private IAssetProvider _assetProvider;
        [Inject] private StuffDataProvider _stuffDataProvider;

        private StuffItemSheet _configs;
        private ManualFormulaSheet<StuffItemSheet, ItemRarityType> _manualOwnedValues;
        private ManualFormulaSheet<StuffItemSheet, string> _manualEquippedValues;
        
        private StuffDataContainer _data;
        private readonly Dictionary<string, StuffSlotType> _slotTypes = new();
        private readonly List<StuffSlot> _slots = new();

        protected override ItemType Type => ItemType.Stuff;

        public IReadOnlyList<StuffSlot> Slots => _slots;
        public event Action<StuffSlot> OnEquip;

        [Preserve]
        public StuffInventory() { }

        public async UniTask WarmUpAsync()
        {
            var locations = await _assetProvider.GetResourceLocationsAsync(AssetLabel.StuffSlot);
            var results = await _assetProvider.WarmUp<ScriptableObject>(locations);
            for (var i = 0; i < locations.Count; i++)
            {
                var slotTag = (StuffSlotType)results[i];
                _slotTypes[slotTag.name] = slotTag;  
            } 
        }

        public override void Initialize()
        {
            _data = _stuffDataProvider.GetData();
            _configs = Balance.Get<StuffItemSheet>();
            _manualOwnedValues = Balance.Get<ManualFormulaSheet<StuffItemSheet, ItemRarityType>>();
            _manualEquippedValues = Balance.Get<ManualFormulaSheet<StuffItemSheet, string>>();
            base.Initialize();
            
            CreateSlots();
        }

        protected override StuffItem CreateItem(ItemData data)
        {
            if(!_configs.Contains(data.Id)) return null;
            var config = _configs[data.Id];
            BaseValueFormula ownedFormula = null;
            if (config.OwnedFormulaType == FormulaType.CustomSheet) 
                ownedFormula = _manualOwnedValues.GetValueFormula(config.RarityType);
            BaseValueFormula equippedFormula = null;
            if (config.EquippedFormulaType == FormulaType.CustomSheet) 
                equippedFormula = _manualEquippedValues.GetValueFormula(config.Id);

            var slotType = _slotTypes[config.SlotType];
            return new StuffItem(Translator, config, data, slotType, Configuration, SpriteProvider.GetSprite(Asset.StuffAtlas, config.ImageName), ownedFormula, equippedFormula);
        }

        private void CreateSlots()
        {
            var sheet = Balance.Get<StuffSlotSheet>();
            foreach (var data in _data.List)
            {
                if (!sheet.Contains(data.Id))
                {
                    Logger.LogError("There is no slot with id " + data.Id + " in the slot sheet.");
                    return;
                }
                var config = sheet[data.Id];
                var tag = _slotTypes[config.Type];
                var slotModel = new StuffSlot(data, config, tag, UnitStatService.GetPlayerStats(), _contentService.GetStuff(config.Id));
                var item = GetItem(data.ItemId);
                if(item != null)
                    slotModel.Equip(item);
                _slots.Add(slotModel);
            }
        }

        public void Equip(StuffItem item, StuffSlot slot)
        {
            if (slot.BaseItem != null) 
                Remove(slot);
            
            slot.Equip(item);
            OnEquip?.Invoke(slot);
        }

        private void Remove(StuffSlot slot) => 
            slot.Remove();

        void IDisposable.Dispose()
        {
            foreach (var slotModel in _slots) 
                slotModel.Dispose();
        }
    }
}