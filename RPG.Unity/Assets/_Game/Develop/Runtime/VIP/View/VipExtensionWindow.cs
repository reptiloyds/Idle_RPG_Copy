using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.VIP.View
{
    public class VipExtensionWindow : BaseWindow
    {
        [SerializeField] private BaseButton _extensionButton;

        [Inject] private IWindowService _windowService;
        
        protected override void Awake()
        {
            base.Awake();
            _extensionButton.OnClick += OnExtensionClick;
        }

        private async void OnExtensionClick()
        {
            _extensionButton.SetInteractable(false);
            await _windowService.OpenAsync<VipWindow>();
            _extensionButton.SetInteractable(true);
            Close();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _extensionButton.OnClick -= OnExtensionClick;
        }
    }
}