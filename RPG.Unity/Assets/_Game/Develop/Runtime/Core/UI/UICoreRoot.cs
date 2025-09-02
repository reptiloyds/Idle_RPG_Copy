using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.PopupNumbers.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Hub.Sides;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Health;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI
{
    public class UICoreRoot : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _healthBarParent;
        [SerializeField, Required] private RectTransform _popupTextParent;
        [SerializeField, Required] private RectTransform _windowOverHudRoot;
        [SerializeField, Required] private RectTransform _windowHudRoot;
        [SerializeField, Required] private RectTransform _windowUnderHudRoot;
        [SerializeField, Required] private RectTransform _overUI;
        [SerializeField, Required] private RectTransform _leftSide;
        [SerializeField, Required] private RectTransform _rightSide;
        [SerializeField, Required] private RectTransform _resourceContainer;
        [SerializeField] private List<PopupResourceSettings> _popupResourceSettings;

        [Inject]
        public void Initialize(HealthBarFactory healthBarFactory, SidesContainer sidesContainer, PopupTextFactory popupTextFactory,
            IPopupResourceFactory popupResourceFactory, IWindowService windowService, ResourceViewService resourceViewService)
        {
            var mainCamera = UnityEngine.Camera.main;
            healthBarFactory.Setup(_healthBarParent, mainCamera);
            popupTextFactory.Setup(_popupTextParent, mainCamera);
            popupResourceFactory.Setup(_popupResourceSettings, mainCamera);
            windowService.Setup(_windowOverHudRoot, _windowHudRoot, _windowUnderHudRoot, _overUI);
            sidesContainer.RegisterSide(UISideType.Left, _leftSide);
            sidesContainer.RegisterSide(UISideType.Right, _rightSide);
            resourceViewService.Setup(_resourceContainer);
        }
    }
}