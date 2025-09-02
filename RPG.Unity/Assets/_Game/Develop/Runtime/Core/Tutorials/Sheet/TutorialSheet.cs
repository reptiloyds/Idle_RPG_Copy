using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Extensions;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Type;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet
{
    public class TutorialSheet : Sheet<string, TutorialRow>
    {
        [Preserve] public TutorialSheet() { }
    }
    
    public class TutorialRow : SheetRowArray<string, TutorialElem>
    {
        [Preserve] public TutorialTriggerType TriggerType { get; private set; }
        [Preserve] public string TriggerJSON { get; private set; }
        [Preserve] public string WarmUpWindows { get; private set; }
        [Preserve] public bool Priority { get; private set; }
        
        [Preserve] public TutorialRow() { }
        
        private List<string> _warmUpWindowsList;
        public List<string> WarmUpWindowsList => _warmUpWindowsList ??= SheetExt.ParseToStringList(WarmUpWindows);

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if (!Application.isPlaying)
                Validate();
        }

        private void Validate() => 
            TriggerType.TryDeserialize(TriggerJSON);
    }

    public class TutorialElem : SheetRowElem
    {
        [Preserve] public int Order { get; private set; }
        [Preserve] public string CloseWindows { get; private set; }
        [Preserve] public float StartDelay { get; private set; }
        [Preserve] public TutorialStepType StepType { get; private set; }
        [Preserve] public string StepJSON { get; private set; }
        [Preserve] public TutorialCompleteCondition CompleteCondition { get; private set; }
        [Preserve] public string ConditionJSON { get; private set; }
        [Preserve] public bool PauseTime { get; private set; }
        [Preserve] public bool PauseSave { get; private set; }
        [Preserve] public string AllowedButtons { get; private set; }
        [Preserve] public bool ForcePopupBlock { get; private set; }

        private List<string> _allowedButtonsList;
        public List<string> AllowedButtonsList => _allowedButtonsList ??= SheetExt.ParseToStringList(AllowedButtons);
        
        private CloseWindowData _closeWindowData;
        public CloseWindowData CloseWindowData => _closeWindowData ??= ParseCloseWindowData(CloseWindows);
        
        [Preserve] public TutorialElem() { }

        public override void PostLoad(SheetConvertingContext context)
        {
            if (!Application.isPlaying)
                Validate();
        }

        private void Validate()
        {
            _closeWindowData = ParseCloseWindowData(CloseWindows);
            _allowedButtonsList = SheetExt.ParseToStringList(AllowedButtons);
            StepType.TryDeserialize(StepJSON);
            CompleteCondition.TryDeserialize(ConditionJSON);
        }
        
        private CloseWindowData ParseCloseWindowData(string json)
        {
            if(json == null) return new CloseWindowData();
            return JsonConvertLog.DeserializeObject<CloseWindowData>(json);
        }
    }
}