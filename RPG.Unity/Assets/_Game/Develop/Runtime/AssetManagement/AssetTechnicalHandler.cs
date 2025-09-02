using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement.Definition;
using PleasantlyGames.RPG.Runtime.AssetManagement.Uploading.View;
using PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.Model;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.View;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.AssetManagement
{
    public class AssetTechnicalHandler : IInitializable, IDisposable
    {
        [Inject] private AssetProviderDefinition _definition;
        private readonly TechnicalMessageService _technicalMessageService;
        private readonly IAssetProvider _assetProvider;
        
        private bool _uploadingMessageIsLoading;
        private bool _uploadingMessageIsActive;
        
        [Preserve]
        public AssetTechnicalHandler(TechnicalMessageService technicalMessageService, IAssetProvider assetProvider)
        {
            _technicalMessageService = technicalMessageService;
            _assetProvider = assetProvider;
        }
        
        public void Initialize()
        {
            _assetProvider.OnLoadingError += OnLoadingError;
            _assetProvider.OnUploadingStart += OnUploadingStart;
            _assetProvider.OnUploadingStop += OnUploadingStop;
            _technicalMessageService.GetAsync<AssetUploadingMessage>().Forget();
        }

        public void Dispose()
        {
            _assetProvider.OnLoadingError -= OnLoadingError;
            _assetProvider.OnUploadingStart -= OnUploadingStart;
            _assetProvider.OnUploadingStop -= OnUploadingStop;
        }

        private async void OnLoadingError() => 
            await _technicalMessageService.Open<ThirdPartyWrongView>();

        private async void OnUploadingStart()
        {
            _uploadingMessageIsActive = true;
            _uploadingMessageIsLoading = true;
            await UniTask.DelayFrame(_definition.FrameDelayBeforeUploadWindow);
            if(!_uploadingMessageIsActive)
                 return;
            await _technicalMessageService.Open<AssetUploadingMessage>();
            _uploadingMessageIsLoading = false;
            if (!_uploadingMessageIsActive)
                _technicalMessageService.Close<AssetUploadingMessage>();   
        }

        private async void OnUploadingStop()
        {
            // await UniTask.DelayFrame(3);
            _uploadingMessageIsActive = false;
            if (_uploadingMessageIsLoading)
                return;
            _technicalMessageService.Close<AssetUploadingMessage>();
        }
    }
}