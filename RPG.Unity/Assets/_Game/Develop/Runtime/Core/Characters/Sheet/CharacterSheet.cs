using System;
using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.Characters.Type;
using PleasantlyGames.RPG.Runtime.Core.Deal.Model;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using PleasantlyGames.RPG.Runtime.Core.Stats.Extension;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Sheet
{
    public class CharacterSheet : Sheet<string, CharacterRow>
    {
        [Preserve] public CharacterSheet() { }
    }
    
    public class CharacterRow : SheetRowArray<string, CharacterRow.Elem> 
    {
        [Preserve] public string LocalizationToken { get; private set; }
        [Preserve] public string BranchIds { get; private set; }
        [Preserve] public CharacterRarityType Rarity { get; private set; }
        [Preserve] public CharacterOwnType OwnType { get; private set; }
        [Preserve] public string PurchaseData { get; private set; }
        [Preserve] public string EvolutionLevelRequests { get; private set; }
        [Preserve] public string Prices { get; private set; }
        [Preserve] public string UnitId { get; private set; }
        [Preserve] public int MaxLevel { get; private set; }
        [Preserve] public FormulaType LevelFormula { get; private set; }
        [Preserve] public string LevelFormulaJSON { get; private set; }
        [Preserve] public string SkillId { get; private set; }
        [Preserve] public string SkillImage { get; private set; }
        [Preserve] public int SkillLevelRequest { get; private set; }

        private List<string> _branches;
        public List<string> Branches => _branches ??= JsonConvertLog.DeserializeObject<List<string>>(BranchIds);
        
        private List<EvolutionData> _evolutions;
        public List<EvolutionData> Evolutions => _evolutions ??= DeserializeEvolution();
        
        [Preserve] public CharacterRow() { }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if (SheetExt.IsValidationNeeded) 
                Validate();
        }

        private void Validate()
        {
            _evolutions = DeserializeEvolution();
            
            _branches = JsonConvertLog.DeserializeObject<List<string>>(BranchIds);
            
            LevelFormula.DeserializeFormula(LevelFormulaJSON);

            switch (OwnType)
            {
                case CharacterOwnType.Free:
                    break;
                case CharacterOwnType.Resource:
                    JsonConvertLog.DeserializeObject<List<ResourcePriceStruct>>(PurchaseData);
                    break;
                case CharacterOwnType.InApp:
                    break;
            }
            
            SheetExt.CheckSprite(Asset.SkillAtlas, SkillImage);
        }

        private List<EvolutionData> DeserializeEvolution()
        {
            var evolutionLevelRequests = JsonConvertLog.DeserializeObject<List<int>>(EvolutionLevelRequests);
            var prices = JsonConvertLog.DeserializeObject<List<List<ResourcePriceStruct>>>(Prices);
            if (prices.Count != evolutionLevelRequests.Count)
            {
                Debug.LogError($"Prices count {prices.Count} != evolutionLevelRequests count {evolutionLevelRequests.Count}");
                return null;
            }
            
            var evolutionList = new List<EvolutionData>(evolutionLevelRequests.Count);
            
            for (var i = 0; i < evolutionList.Capacity; i++)
            {
                var evolutionData = new EvolutionData
                {
                    Level = evolutionLevelRequests[i],
                    Price = prices[i]
                };
                evolutionList.Add(evolutionData);
            }

            return evolutionList;
        }

        public class Elem : SheetRowElem 
        {
            [Preserve] public string ImageName { get; private set; }
            [Preserve] public BonusConditionType BonusConditionType { get; private set; }
            [Preserve] public string BonusConditionJSON { get; private set; }
            [Preserve] public StatType StatType { get; private set; }
            [Preserve] public string EffectType { get; private set; }
            [Preserve] public StatModType EffectModType { get; private set; } 
            [Preserve] public FormulaType EffectFormulaType { get; private set; }
            [Preserve] public string EffectFormulaJSON { get; private set; }

            [Preserve] public Elem() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);

                if (!Application.isPlaying) 
                    Validate();
            }

            private void Validate()
            {
                BonusConditionType.DeserializeBonus(BonusConditionJSON);
                
                switch (StatType)
                {
                    case StatType.Unit:
                        var unitStatType = GetEffectType<UnitStatType>();
                        if (unitStatType is UnitStatType.None) 
                            Debug.LogError("Invalid unit stat type: " + EffectType);
                        break;
                    case StatType.Global:
                        var globalStatType = GetEffectType<GlobalStatType>();
                        if (globalStatType is GlobalStatType.None)
                            Debug.LogError("Invalid global stat type: " + EffectType);
                        break;
                }
                
                EffectFormulaType.DeserializeFormula(EffectFormulaJSON);
                
                SheetExt.CheckSprite(Asset.MainAtlas, ImageName);
            }
            
            public T GetEffectType<T>() where T : struct, Enum
            {
                Enum.TryParse<T>(EffectType, ignoreCase: true, out var statType);
                return statType;
            }
        }
    }
}