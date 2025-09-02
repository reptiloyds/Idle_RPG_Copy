using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using PrimeTween;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.SwitchBranch
{
    internal class SwitchBranchTutorialStep : TutorialStep
    {
        private readonly SwitchBranchTutorialData _data;
        [Inject] private BranchService _branchService;

        public SwitchBranchTutorialStep(TutorialElem config) : base(config) => 
            _data = JsonConvert.DeserializeObject<SwitchBranchTutorialData>(config.StepJSON);

        public override void Start()
        {
            base.Start();
            foreach (var branch in _branchService.Branches)
            {
                if (!string.Equals(branch.Id, _data.BranchId)) continue;
                _branchService.ChangeBranch(branch);
                break;
            }

            Tween.Delay(0.1f, Complete, useUnscaledTime: true);
        }
    }
}