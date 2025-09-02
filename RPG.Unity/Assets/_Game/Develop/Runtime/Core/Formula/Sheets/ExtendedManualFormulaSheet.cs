using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Formula.Sheets
{
    public class ExtendedManualFormulaSheet<TRef, TKey1, TKey2> :
        Sheet<ExtendedManualFormulaSheet<TRef, TKey1, TKey2>.Row>
        where TRef : class
    {
        private readonly Dictionary<TKey1, Dictionary<TKey2, ManualBigDoublesFormula>> _cache = new();

        [Preserve]
        public ExtendedManualFormulaSheet()
        {
        }

        public BaseValueFormula GetValueFormula(TKey1 primaryKey, TKey2 secondKey)
        {
            if (_cache.TryGetValue(primaryKey, out var formulaDictionary) &&
                formulaDictionary.TryGetValue(secondKey, out var formula))
                return formula;

            _cache[primaryKey] = new Dictionary<TKey2, ManualBigDoublesFormula>();

            var manualDataList = new List<ManualData>(Count);
            foreach (var row in this)
            {
                if (!row.Dictionary.TryGetValue(primaryKey, out var dictionary)) continue;
                if (!dictionary.TryGetValue(secondKey, out var values)) continue;
                if(values.Count != 2) continue;
                manualDataList.Add(new ManualData(values[0], (long)values[1]));
            }

            var result = new ManualBigDoublesFormula(manualDataList);
            _cache[primaryKey][secondKey] = result;

            return result;
        }

        public class Row : SheetRow
        {
            [Preserve] public Dictionary<TKey1, Dictionary<TKey2, List<float>>> Dictionary { get; private set; }

            [Preserve]
            public Row()
            {
            }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);

                if (SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                foreach (var secondDic in Dictionary)
                foreach (var kvp in secondDic.Value)
                    if (kvp.Value.Count != 0 && kvp.Value.Count != 2)
                        Debug.LogError($"{secondDic.Key}_{kvp.Key}_{Id} has wrong values");
            }
        }
    }
}