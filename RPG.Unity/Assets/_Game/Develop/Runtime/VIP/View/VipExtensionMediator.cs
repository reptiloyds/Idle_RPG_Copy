using System;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using R3;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.VIP.View
{
    public class VipExtensionMediator : IInitializable, IDisposable
    {
        [Inject] private VipService _vipService;
        [Inject] private IWindowService _windowService;

        void IInitializable.Initialize()
        {
            _vipService.ExtensionRequired
                .Where(value => value)
                .Subscribe(_ => OnExtensionRequested());
        }

        private async void OnExtensionRequested()
        {
            await _windowService.PushPopupAsync<VipExtensionWindow>();
            _vipService.OnExtensionOffered();
        }

        void IDisposable.Dispose()
        {
            
        }
    }
}