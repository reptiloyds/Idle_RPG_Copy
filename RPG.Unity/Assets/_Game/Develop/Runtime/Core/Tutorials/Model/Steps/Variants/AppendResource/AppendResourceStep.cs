using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.AppendResource
{
    internal class AppendResourceStep : TutorialStep
    { 
        private readonly AppendResourceData _data;

        [Inject] private ResourceService _resourceService;
        
        public AppendResourceStep(TutorialElem config) : base(config) => 
            _data = JsonConvert.DeserializeObject<AppendResourceData>(config.StepJSON);

        public override void Start()
        {
            base.Start();
            
            _resourceService
                .GetResource(_data.ResourceType)
                .Add(_data.Amount);
        }
    }
}