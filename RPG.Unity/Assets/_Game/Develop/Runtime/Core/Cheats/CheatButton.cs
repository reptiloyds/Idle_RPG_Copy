using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Cheats
{
    public class CheatButton : BaseButton
    {
        [Inject] private CheatService _cheatService;
        [Inject] private IWindowService _windowService;

        private void Start()
        {
            gameObject.SetActive(_cheatService.IsEnabled);
        }

        protected override async void Click()
        {
            base.Click();
            await _windowService.OpenAsync<CheatWindow>();
        }
    }
}