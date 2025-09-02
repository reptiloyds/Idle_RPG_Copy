using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.UI.Mediator
{
    public class UIGameModeResultMediator : IInitializable, IDisposable
    {
        [Inject] private IWindowService _windowService;
        [Inject] private IEnumerable<IGameMode> _modes;
        [Inject] private IAudioService _audioService;

        private IGameMode _lastMode;

        [Preserve]
        public UIGameModeResultMediator()
        {
        }

        void IInitializable.Initialize()
        {
            foreach (var mode in _modes)
            {
                mode.OnWin += OnWin;
                mode.OnLose += OnLose;
            }
        }

        void IDisposable.Dispose()
        {
            foreach (var mode in _modes)
            {
                mode.OnWin -= OnWin;
                mode.OnLose -= OnLose;
            }
        }

        public void OnResultWasPresented() =>
            _lastMode.OnResultPresented();

        public void OnResultWasClosed() =>
            _lastMode.OnResultWasClosed();

        private async void OnWin(IGameMode mode)
        {
            _lastMode = mode;
            var window = await _windowService.OpenAsync<GameModeResultWindow>();
            window.SetVictoryVisual();
            _audioService
                .CreateLocalSound(SFX_Common.SFX_Common_Win)
                .Play();
        }

        private async void OnLose(IGameMode mode)
        {
            _lastMode = mode;
            var window = await _windowService.OpenAsync<GameModeResultWindow>();
            window.SetDefeatVisual();
            _audioService
                .CreateLocalSound(UI_Effect.UI_Lose)
                .Play();
        }
    }
}