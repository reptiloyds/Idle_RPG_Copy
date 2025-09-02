using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Type;
using PleasantlyGames.RPG.Runtime.Core.Skill.Extension;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Data;
using PleasantlyGames.RPG.Runtime.Core.Skill.Type;
using PleasantlyGames.RPG.Runtime.Core.Skill.View.Data;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Sheet
{
    public class SkillSheet : Sheet<string, SkillRow>
    {
        [Preserve] public SkillSheet() { }
    }
    
    public class SkillRow : SheetRowArray<string, SkillRow.Elem>
    {
        [Preserve] public float Cooldown { get; private set; }
        [Preserve] public SkillEffectType EffectType { get; private set; }
        [Preserve] public float Delay { get; private set; }
        [Preserve] public string EffectDataJSON { get; private set; }
        [Preserve] public string ViewJSON { get; private set; }
        
        private List<SkillViewData> _views;
        public List<SkillViewData> Views => _views ??= DeserializeViewData(ViewJSON);

        [Preserve] public SkillRow() { }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if (SheetExt.IsValidationNeeded)
                Validate();
        }

        private void Validate()
        {
            EffectType.TryDeserialize(EffectDataJSON);
            _views = DeserializeViewData(ViewJSON);
            
#if UNITY_EDITOR
            foreach (var viewData in _views) 
                SheetExt.CheckAsset(viewData.Key);
#endif
            if (_views.Count != EffectType.GetViewAmount()) 
                Debug.LogError($"View count mismatch: {_views.Count} != {EffectType.GetViewAmount()}");
        }

        private List<SkillViewData> DeserializeViewData(string json) => 
            JsonConvertLog.DeserializeObject<List<SkillViewData>>(json);

        public class Elem : SheetRowElem
        {
            [Preserve] public SkillValueType ValueType { get; private set; }
            [Preserve] public FormulaType ValueFormulaType { get; private set; }
            [Preserve] public string ValueFormulaJSON { get; private set; }
            
            [Preserve] public Elem() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                if (ValueFormulaType != FormulaType.CustomSheet) 
                    ValueFormulaType.DeserializeFormula(ValueFormulaJSON);
            }
        }
    }
}