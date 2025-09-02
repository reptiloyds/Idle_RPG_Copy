using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Mediator
{
    public class UIDungeonWinMediator<TDungeon, TWinWindow> : IInitializable, IDisposable where TDungeon : DungeonMode where TWinWindow : BaseWindow
    {
        [Inject] private TDungeon _mode;
        [Inject] private IWindowService _windowService;
        
        [Preserve]
        public UIDungeonWinMediator()
        {
            
        }
        
        public void Initialize() => 
            _mode.OnWin += OnWin;

        public void Dispose() => 
            _mode.OnWin -= OnWin;

        private async void OnWin(IGameMode gameMode) => 
            await _windowService.OpenAsync<TWinWindow>();
    }
}