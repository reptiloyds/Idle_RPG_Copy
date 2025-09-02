using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Collections.Save;
using PleasantlyGames.RPG.Runtime.Core.Collections.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Collections.Model
{
    public class CollectionService : IDisposable
    {
        [Inject] private BalanceContainer _balance;
        [Inject] private CollectionDataProvider _dataProvider;
        [Inject] private ITranslator _translator;
        [Inject] private BranchService _branchService;
        [Inject] private UnitStatService _unitStatService;
        
        [Inject] private BaseInventory<StuffItem> _stuffInventory;
        [Inject] private BaseInventory<SkillItem> _skillInventory;
        [Inject] private BaseInventory<CompanionItem> _companionInventory;
        
        private CollectionDataContainer _dataContainer;
        private readonly List<Collection> _collections = new();
        
        public IReadOnlyList<Collection> Collections => _collections;

        [Preserve]
        public CollectionService()
        {
        }
        
        public void Initialize()
        {
            _dataContainer = _dataProvider.GetData();
            CreateCollections();
        }

        void IDisposable.Dispose()
        {
            foreach (var collection in _collections) 
                collection.Dispose();
        }

        private void CreateCollections()
        {
            var sheet = _balance.Get<CollectionSheet>();
            var manualValueSheet = _balance.Get<ManualFormulaSheet<CollectionSheet, string>>();
            foreach (var data in _dataContainer.List)
            {
                var config = sheet[data.Id];
                var items = GetCollectionItems(config.Type, config.ItemIds);
                var stat = _unitStatService.GetPlayerStat(config.EffectType);
                BaseValueFormula formula = null;
                if (config.EnhanceFormulaType == FormulaType.CustomSheet)
                    formula = manualValueSheet.GetValueFormula(config.Id);
                else
                    formula = config.EnhanceFormulaType.CreateFormula(config.EnhanceFormulaJSON);
                var collection = new Collection(items, stat, _translator, config, data, formula);
                _collections.Add(collection);
            }
        }

        private List<Item> GetCollectionItems(ItemType type, HashSet<string> ids)
        {
            var result = new List<Item>(ids.Count);
            switch (type)
            {
                case ItemType.Stuff:
                    Filter(_stuffInventory.Items, ids, result);
                    break;
                case ItemType.Companion:
                    Filter(_companionInventory.Items, ids, result);
                    break;
                case ItemType.Skill:
                    Filter(_skillInventory.Items, ids, result);
                    break;
            }

            return result;
        }

        private void Filter<T>(IReadOnlyList<T> items, HashSet<string> ids, List<Item> result) where T : Item
        {
            foreach (var item in items)
                if (ids.Contains(item.Id)) 
                    result.Add(item);
        }

        public void Enhance(Collection collection) => 
            collection.Enhance();
    }
}