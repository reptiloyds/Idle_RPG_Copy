using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Button
{
    public class ButtonTutorialTrigger : TutorialTrigger
    {
        private readonly IButtonService _buttonService;
        private readonly ButtonTutorialData _data;
        
        public ButtonTutorialTrigger(IButtonService buttonService, string dataJSON)
        {
            _buttonService = buttonService;
            _data = JsonConvert.DeserializeObject<ButtonTutorialData>(dataJSON);
        }

        public override void Initialize() => 
            _buttonService.OnButtonIdClick += OnButtonIdClick;

        public override void Dispose() => 
            _buttonService.OnButtonIdClick -= OnButtonIdClick;

        private void OnButtonIdClick(string buttonId)
        {
            if(!string.Equals(buttonId, _data.ButtonId)) return;
            Execute();
        }
    }
}