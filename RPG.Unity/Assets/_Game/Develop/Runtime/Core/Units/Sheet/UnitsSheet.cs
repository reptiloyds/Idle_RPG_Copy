using System;
using System.Collections.Generic;
using System.Linq;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Config;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Sheet
{
    public class UnitsSheet : Sheet<string, UnitsSheet.Row>
    {
        [Preserve] public UnitsSheet() { }
        
        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            VerifyReference();
        }

        private void VerifyReference()
        {
            foreach (var unit in this)
            {
                if(unit.StatReference.IsNullOrWhitespace()) continue;
                var referenceStats = new VerticalList<Elem>(this[unit.StatReference].Arr);

                for (var i = 0; i < referenceStats.Count; i++)
                {
                    var sameStat = unit.FirstOrDefault(item => item.StatType == referenceStats[i].StatType);
                    if(sameStat == null) continue;
                    referenceStats[i] = sameStat;
                }
                unit.Arr.Clear();
                unit.Arr.AddRange(referenceStats);
                unit.RemoveReference();
            }
        }

        public class Row : SheetRowArray<string, Elem>
        {
            [Preserve] public string TypeTagsJSON { get; private set; }
            [Preserve] public string PrefabIdsJSON { get; private set; }
            [Preserve] public string ImagesJSON { get; private set; }
            [Preserve] public string MainImage { get; private set; }
            [Preserve] public string StatReference { get; private set; }

            private List<string> _prefabIds;
            protected List<string> PrefabIds => _prefabIds ??= DeserializeList<string>(PrefabIdsJSON);
            
            private List<string> _images;
            public List<string> Images => _images ??= DeserializeList<string>(ImagesJSON, true);

            private List<UnitSubType> _typeTags;
            public List<UnitSubType> TypeTags => _typeTags ??= DeserializeList<UnitSubType>(TypeTagsJSON);

            [Preserve] public Row() { }

            public void RemoveReference() => 
                StatReference = null;

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if (SheetExt.IsValidationNeeded) 
                    Validate();
            }

            public string GetPrefabId(int index) => 
                PrefabIds[Math.Min(PrefabIds.Count - 1, index)];

            private void Validate()
            {
                _typeTags = DeserializeList<UnitSubType>(TypeTagsJSON, true);
                _prefabIds = DeserializeList<string>(PrefabIdsJSON);
                foreach (var prefabId in _prefabIds) 
                    SheetExt.CheckAsset(prefabId ?? Id);
                
                _images = DeserializeList<string>(ImagesJSON, true);
                foreach (var image in _images) 
                    SheetExt.CheckSprite(Asset.UnitAtlas, image);
            }

            private List<T> DeserializeList<T>(string json, bool possibleNull = false)
            {
                if (json.IsNullOrWhitespace())
                {
                    var message = "Json is null or whitespace";
                    if (!possibleNull)
                    {
                        Debug.LogError(message);
                        return null;
                    }
                    return new List<T>(0);
                }

                return JsonConvertLog.DeserializeObject<List<T>>(json);
            }
        }

        public class Elem : SheetRowElem
        {
            [Preserve] public UnitStatType StatType { get; private set; }
            [Preserve] public FormulaType ValueFormulaType { get; private set; }
            [Preserve] public string ValueFormulaJSON { get; private set; }
            [Preserve] public int MaxLevel { get; private set; }
            [Preserve] public string ImageName { get; private set; }
            
            public UnitStatConfig Config { get; private set; }

            [Preserve] public Elem() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                Config = new UnitStatConfig()
                {
                    StatType = StatType,
                    ValueFormulaType = ValueFormulaType,
                    ValueFormulaJSON = ValueFormulaJSON,
                    MaxLevel = MaxLevel,
                    ImageName = ImageName,
                };

                if (SheetExt.IsValidationNeeded) 
                    Validate();
            }

            private void Validate()
            {
                ValueFormulaType.DeserializeFormula(ValueFormulaJSON);

                if (MaxLevel < 0) 
                    Debug.LogError("MaxLevel is less than 0");
                if (MaxLevel == 0) MaxLevel = 1;
                
                if(!string.IsNullOrEmpty(ImageName))
                    SheetExt.CheckSprite(Asset.MainAtlas, ImageName);
            }
        }
    }
}