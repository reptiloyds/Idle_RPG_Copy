using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.TutorialAnimation;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.PlayAnimation
{
    internal class PlayAnimationStep : TutorialStep
    {
        private readonly PlayAnimationData _data;

        [Inject] private TutorialAnimationLauncher _animationLauncher;
        
        public PlayAnimationStep(TutorialElem config) : base(config) => 
            _data = JsonConvert.DeserializeObject<PlayAnimationData>(config.StepJSON);

        public override void Start()
        {
            base.Start();
            
            _animationLauncher.Play(_data.ButtonId, Complete);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _animationLauncher.Stop(_data.ButtonId);
        }
        
        public override void Pause()
        {
            base.Pause();
            
            _animationLauncher.Stop(_data.ButtonId);
        }

        public override void Continue()
        {
            base.Continue();
            
            _animationLauncher.Play(_data.ButtonId, Complete);
        }
    }
}