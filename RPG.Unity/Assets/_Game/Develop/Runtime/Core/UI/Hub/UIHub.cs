using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Hub.Buttons;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Hub
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UIHub : MonoBehaviour, IInitializable
    {
        [SerializeField] private List<HubButton> _buttons = new();
        [ShowInInspector, HideInEditorMode, ReadOnly] private HubButton _currentButton;

        [Inject] private DungeonModeFacade _dungeonModeFacade;

        void IInitializable.Initialize()
        {
            foreach (var button in _buttons)
            {
                button.OnAutoActivate += OnAutoActivate;
                button.OnAutoDeactivate += OnAutoDeactivate;
            } 
            _dungeonModeFacade.OnLaunched += OnDungeonLaunched;
            _dungeonModeFacade.OnDisposed += OnDungeonDisposed;
            if(_dungeonModeFacade.IsLaunched)
                Disable();
            else
                Enable();
        }

        private void OnDestroy()
        {
            foreach (var button in _buttons)
            {
                button.OnAutoActivate -= OnAutoActivate;
                button.OnAutoDeactivate -= OnAutoDeactivate;
            }
            _dungeonModeFacade.OnLaunched -= OnDungeonLaunched;
            _dungeonModeFacade.OnDisposed -= OnDungeonDisposed;
        }

        private void OnDungeonLaunched(DungeonMode mode) => 
            Disable();

        private void OnDungeonDisposed(DungeonMode mode) => 
            Enable();

        private void OnAutoActivate(HubButton button)
        {
            if (_currentButton != null && _currentButton != button) 
                _currentButton.Deactivate();
            _currentButton = button;
        }

        private void OnAutoDeactivate(HubButton button)
        {
            if (_currentButton != null && _currentButton != button) 
                _currentButton = null;
        }

        private void Enable() => 
            gameObject.SetActive(true);

        private void Disable() => 
            gameObject.SetActive(false);
    }
}