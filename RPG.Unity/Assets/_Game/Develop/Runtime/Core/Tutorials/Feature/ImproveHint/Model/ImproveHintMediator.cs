using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.ImproveHint.View;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.ImproveHint.Model
{
    public class ImproveHintMediator : IInitializable, IDisposable, ITickable
    {
        [Inject] private MainMode _mainMainMode;
        [Inject] private IWindowService _windowService;

        private ImproveHintWindow _window;

        private const float OpenWindowDelay = 0.75f;
        private bool _isTimerActive;
        private float _timer;

        [Preserve]
        public ImproveHintMediator() { }

        public void Initialize() => 
            _mainMainMode.OnLose += OnLose;

        public void Dispose() => 
            _mainMainMode.OnLose -= OnLose;

        private async void OnLose(IGameMode gameMode)
        {
            if(_windowService.IsOpen<ImproveHintWindow>()) return;
            if(_window == null)
                _window = await _windowService.GetAsync<ImproveHintWindow>(false);
            _isTimerActive = true;
            _timer = OpenWindowDelay;
        }

        public void Tick()
        {
            if(!_isTimerActive) return;
            _timer -= Time.deltaTime;
            if (_timer > 0) return;
            
            _isTimerActive = false;
            _window.Open();
        }
    }
}