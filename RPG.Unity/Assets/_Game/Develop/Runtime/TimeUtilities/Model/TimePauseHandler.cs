using System;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.TimeUtilities.Model
{
    public sealed class TimePauseHandler : IInitializable, IDisposable
    {
        [Inject] private IPauseService _pauseService;

        [Preserve]
        public TimePauseHandler() { }
        
        void IInitializable.Initialize()
        {
            _pauseService.OnPause += OnPause;
            _pauseService.OnContinue += OnContinue;
        }

        void IDisposable.Dispose()
        {
            _pauseService.OnPause -= OnPause;
            _pauseService.OnContinue -= OnContinue;
        }

        private void OnPause(PauseType pauseType)
        {
            if(pauseType != PauseType.Time) return;
            Time.timeScale = 0;
        }

        private void OnContinue(PauseType pauseType)
        {
            if(pauseType != PauseType.Time) return;
            Time.timeScale = 1;
        }
    }
}