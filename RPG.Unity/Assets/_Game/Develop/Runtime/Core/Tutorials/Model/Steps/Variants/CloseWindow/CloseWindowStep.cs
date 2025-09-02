using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.CloseWindow
{
    internal class CloseWindowStep : TutorialStep
    {
        private readonly CloseWindowData _data;
        
        public CloseWindowStep(TutorialElem config) : base(config) => 
            _data = JsonConvert.DeserializeObject<CloseWindowData>(config.StepJSON);

        public override void Start()
        {
            base.Start();
            WindowService.Close(_data.WindowId);
        }
    }
}