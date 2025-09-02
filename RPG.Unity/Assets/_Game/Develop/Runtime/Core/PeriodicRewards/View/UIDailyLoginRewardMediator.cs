using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.View
{
    public class UIDailyLoginRewardMediator
    {
        [Inject] private IWindowService _windowService;
        [Inject] private PeriodicRewardService _periodicRewardService;

        private PeriodicRewardsModel _model;
        
        [Preserve]
        public UIDailyLoginRewardMediator() { }

        public async UniTask InitializeAsync()
        {
            _model = _periodicRewardService.GetModel(PeriodicRewardVariant.DailyLogin);
            if (_model.IsRewardReady) 
                await _windowService.PushPopupAsync<DailyLoginRewardWindow>();
        }
    }
}