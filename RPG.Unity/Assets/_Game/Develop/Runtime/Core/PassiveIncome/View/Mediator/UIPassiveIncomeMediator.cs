using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PassiveIncome.View.Mediator
{
    public class UIPassiveIncomeMediator
    {
        [Inject] private PassiveIncomeModel _model;
        [Inject] private IWindowService _windowService;
        
        [Preserve]
        public UIPassiveIncomeMediator()
        {
        }
        
        public async UniTask InitializeAsync()
        {
            if (_model.IsRewardReady) 
                await _windowService.PushPopupAsync<PassiveIncomeWindow>();
        }
    }
}