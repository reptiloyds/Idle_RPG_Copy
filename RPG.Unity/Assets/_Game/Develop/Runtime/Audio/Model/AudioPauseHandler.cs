using System;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Audio.Model
{
    public sealed class AudioPauseHandler : IInitializable, IDisposable
    {
        private readonly IPauseService _pauseService;

        [Preserve]
        [Inject]
        public AudioPauseHandler(IPauseService pauseService) => 
            _pauseService = pauseService;

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
            if(pauseType != PauseType.Audio) return;
            
            AudioListener.pause = true;
        }

        private void OnContinue(PauseType pauseType)
        {
            if(pauseType != PauseType.Audio) return;
            
            AudioListener.pause = false;
        }
    }
}