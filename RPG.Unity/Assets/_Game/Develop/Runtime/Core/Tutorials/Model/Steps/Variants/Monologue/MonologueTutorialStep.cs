using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Monologue.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Monologue.View;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Monologue
{
    internal class MonologueTutorialStep : TutorialStep
    {
        [Inject] private MonologueService _monologueService;
        private readonly MonologueTutorialData _data;
        
        internal MonologueTutorialStep(TutorialElem config) : base(config) => 
            _data = JsonConvert.DeserializeObject<MonologueTutorialData>(config.StepJSON);

        public override void Start()
        {
            base.Start();
            
            _monologueService.Add(_data);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _monologueService.Remove(_data);
        }

        public override void Pause()
        {
            base.Pause();
            
            _monologueService.Remove(_data);
        }

        public override void Continue()
        {
            base.Continue();
            
            _monologueService.Add(_data);
        }
    }
}