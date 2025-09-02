using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Accent.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Accent
{
    internal class AccentTutorialStep : TutorialStep
    {
        private readonly AccentTutorialData _data;

        [Inject] private AccentService _accentService;
        
        internal AccentTutorialStep(TutorialElem config) : base(config) => 
            _data = JsonConvert.DeserializeObject<AccentTutorialData>(config.StepJSON);

        public override void Start()
        {
            base.Start();

            _accentService.Add(_data);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _accentService.Remove(_data);
        }

        public override void Pause()
        {
            base.Pause();
            
            _accentService.Remove(_data);
        }

        public override void Continue()
        {
            base.Continue();
            
            _accentService.Add(_data);
        }
    }
}