using System;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.AssetManagement
{
    public partial class AssetProvider
    {
        [Inject] private IPauseService _pauseService;

        private int _uploadCounter;
        private bool _isActive;

        public event Action OnUploadingStart;
        public event Action OnUploadingStop;

        private void StartUpload()
        {
            _uploadCounter++;
            if (_isActive) return;

            _isActive = true;
            _pauseService.Pause(PauseType.Time);
            _pauseService.Pause(PauseType.UIInput);
            OnUploadingStart?.Invoke();
        }

        private void StopUpload()
        {
            _uploadCounter--;
            if (_uploadCounter > 0 || !_isActive) return;

            _isActive = false;
            _pauseService.Continue(PauseType.Time);
            _pauseService.Continue(PauseType.UIInput);
            OnUploadingStop?.Invoke();
        }
    }
}