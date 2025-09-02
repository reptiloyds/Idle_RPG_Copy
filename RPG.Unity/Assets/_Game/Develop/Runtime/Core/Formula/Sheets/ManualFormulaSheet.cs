using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Formula.Sheets
{
    public class ManualFormulaSheet<TRef, TKey> : Sheet<ManualFormulaSheet<TRef, TKey>.Row> where TRef : class
    {
        private readonly Dictionary<TKey, ManualBigDoublesFormula> _cache = new();
        
        [Preserve]
        public ManualFormulaSheet() { }

        public BaseValueFormula GetValueFormula(TKey key)
        {
            if (_cache.TryGetValue(key, out var formula))
                return formula;

            var manualDataList = new List<ManualData>(Count);
            foreach (var row in this)
            {
                if (!row.Dictionary.TryGetValue(key, out var value)) continue;
                if(value.Count != 2) continue;
                manualDataList.Add(new ManualData(value[0], (long)value[1]));
            }

            formula = new ManualBigDoublesFormula(manualDataList);
            _cache[key] = formula;

            return formula;
        }
        
        public class Row : SheetRow
        {
            [Preserve] public Dictionary<TKey, List<float>> Dictionary { get; private set; }

            [Preserve]
            public Row() { }
            
            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);

                if (SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                foreach (var secondDic in Dictionary)
                    if (secondDic.Value.Count != 0 && secondDic.Value.Count != 2)
                        Debug.LogError($"{secondDic.Key}_{Id} has wrong values");
            }
        }
    }
}