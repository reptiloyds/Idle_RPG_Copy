using System;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet
{
    public class BlessingElem : SheetRowElem
    {
        [Preserve] public StatType StatType { get; private set; }
        [Preserve] public string EffectType { get; private set; }
        [Preserve] public StatModType EffectModType { get; private set; } 
        [Preserve] public FormulaType EffectFormulaType { get; private set; }
        [Preserve] public string EffectFormulaJSON { get; private set; }
        [Preserve] public bool ZeroOnFirstLevel { get; private set; }
            
        [Preserve] public BlessingElem() { }
        
        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);
                
            if(SheetExt.IsValidationNeeded)
                Validate();
        }

        private void Validate()
        {
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
        }

        public T GetEffectType<T>() where T : struct, Enum
        {
            Enum.TryParse<T>(EffectType, ignoreCase: true, out var statType);
            return statType;
        }
    }
}