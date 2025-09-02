using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.OpenWindow
{
    internal class OpenWindowStep : TutorialStep
    {
        private readonly OpenWindowData _data;
        
        public OpenWindowStep(TutorialElem config) : base(config) => 
            _data = JsonConvert.DeserializeObject<OpenWindowData>(config.StepJSON);

        public override void Start()
        {
            base.Start();
            WindowService.OpenAsync(_data.WindowId);
        }
    }
}