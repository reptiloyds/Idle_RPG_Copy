using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Hub.Buttons
{
    public abstract class WindowHubButton<T> : HubButton where T : BaseWindow
    {
        [SerializeField] private string _windowId;
        
        [Inject] private IWindowService _windowService;

        private void Start()
        {
            _windowService.OnOpen += OnWindowOpen;
            _windowService.OnClose += OnWindowClose;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _windowService.OnOpen -= OnWindowOpen;
            _windowService.OnClose -= OnWindowClose;
        }

        protected override async void Click()
        {
            base.Click();

            if (IsActive)
                _windowService.Close<T>();
            else
                await _windowService.OpenAsync<T>();
        }

        private void OnWindowOpen(BaseWindow window)
        {
            if(!string.Equals(window.Id, _windowId)) return;
            
            IsActive = true;
            EnableCloseVisual();
            TriggerAutoActivate();
        }

        private void OnWindowClose(BaseWindow window)
        {
            if(!string.Equals(window.Id, _windowId)) return;
            
            IsActive = false;
            DisableCloseVisual();
            TriggerAutoDeactivate();
        }

        public override void Deactivate() => 
            _windowService.Close<T>();
    }
}