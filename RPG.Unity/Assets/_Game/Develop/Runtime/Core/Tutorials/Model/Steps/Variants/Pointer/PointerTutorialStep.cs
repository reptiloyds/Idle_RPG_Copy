using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.View;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Pointer
{
    internal class PointerTutorialStep : TutorialStep
    {
        private readonly PointerTutorialData _data;

        [Inject] private PointerService _presenter;
        
        internal PointerTutorialStep(TutorialElem config) : base(config) => 
            _data = JsonConvert.DeserializeObject<PointerTutorialData>(config.StepJSON);

        public override void Start()
        {
            _presenter.Add(_data);
            
            base.Start();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _presenter.Remove(_data);
        }

        public override void Pause()
        {
            base.Pause();
         
            _presenter.Remove(_data);
        }

        public override void Continue()
        {
            base.Continue();
            
            _presenter.Add(_data);
        }
    }
}