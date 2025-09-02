using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.MilitaryBase.View
{
    public class MilitaryBaseOpenButton : BaseButton
    {
        [Inject] private IWindowService _windowService;

        protected override async void Click()
        {
            base.Click();

            await _windowService.OpenAsync<MilitaryBaseWindow>();
        }
    }
}