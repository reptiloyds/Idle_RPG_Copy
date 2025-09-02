using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.Model;
using PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.Model;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.View
{
    public class InternetConnectionMediator : IInitializable, IDisposable
    {
        [Inject] private InternetConnectionService _service;
        [Inject] private TechnicalMessageService _technicalMessageService;
        [Inject] private IPauseService _pauseService;

        private bool _isOpened;

        [Preserve]
        public InternetConnectionMediator()
        {
            
        }

        public void Initialize()
        {
            _service.OnConnectionLost += OnConnectionLost;
            _service.OnConnectionRestored += OnConnectionRestored;
        }

        public void Dispose()
        {
            _service.OnConnectionLost -= OnConnectionLost;
            _service.OnConnectionRestored -= OnConnectionRestored;
        }

        private async void OnConnectionLost()
        {
            _pauseService.Pause(PauseType.Time);
            await _technicalMessageService.Open<InternetConnectionLostView>();
            if (_service.HasConnection) 
                _technicalMessageService.Close<InternetConnectionLostView>();
        }

        private void OnConnectionRestored()
        {
            _technicalMessageService.Close<InternetConnectionLostView>();
            _pauseService.Continue(PauseType.Time);
        }
    }
}