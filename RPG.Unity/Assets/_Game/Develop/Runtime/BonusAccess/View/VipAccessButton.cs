using PleasantlyGames.RPG.Runtime.BonusAccess.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.VIP.Contract;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using PleasantlyGames.RPG.Runtime.VIP.View;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.BonusAccess.View
{
    public partial class BonusAccessButton
    {
        [Inject] private VipService _vipService;
        [Inject] private IWindowService _windowService;

        private void InitializeVipAccess()
        {
            _vipService.IsActive
                .Subscribe(_ => UpdateState())
                .AddTo(this);
        }

        private void DisposeVipAccess()
        {
        }

        private void TryEnterVipState()
        {
            if (_vipService.IsActive.CurrentValue)
                EnterState(BonusButtonState.FreeAccess);
            else
            {
                EnterState(BonusButtonState.Vip);
                SetInteractable(CanExecute());
            }
        }

        private async void ClickOnVip()
        {
            SetInteractable(false);
            await _windowService.OpenAsync<VipWindow>();
            SetInteractable(true);
        }
    }
}