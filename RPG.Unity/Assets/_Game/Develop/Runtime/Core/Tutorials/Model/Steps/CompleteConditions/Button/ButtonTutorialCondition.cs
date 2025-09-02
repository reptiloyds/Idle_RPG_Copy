using System.Collections.Generic;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Button
{
    public class ButtonTutorialCondition : TutorialCondition
    {
        private readonly IButtonService _buttonService;
        private readonly HashSet<string> _buttonIds;

        public ButtonTutorialCondition(IButtonService buttonService, string json)
        {
            _buttonService = buttonService;
            _buttonIds = JsonConvert.DeserializeObject<HashSet<string>>(json);
        }
        
        public override void Initialize()
        {
            base.Initialize();
            
            _buttonService.OnButtonIdClick += OnButtonIdClick;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _buttonService.OnButtonIdClick -= OnButtonIdClick;
        }

        public override void Pause() => 
            _buttonService.OnButtonIdClick -= OnButtonIdClick;

        public override void Continue() => 
            _buttonService.OnButtonIdClick += OnButtonIdClick;

        private void OnButtonIdClick(string buttonId)
        {
            if(!_buttonIds.Contains(buttonId)) return;

            _buttonService.OnButtonIdClick -= OnButtonIdClick;
            Complete();
        }
    }
}