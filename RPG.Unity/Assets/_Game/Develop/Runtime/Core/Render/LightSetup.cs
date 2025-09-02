using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Device;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using DeviceType = PleasantlyGames.RPG.Runtime.Device.DeviceType;

namespace PleasantlyGames.RPG.Runtime.Core.Render
{
    [DisallowMultipleComponent, HideMonoScript]
    public sealed class LightSetup : MonoBehaviour
    {
        [SerializeField] private List<Light> _lights;
        
        [Inject]
        private void Construct(IDeviceService deviceService)
        {
            if (deviceService.GetDevice() == DeviceType.Mobile)
            {
                foreach (var lightComponent in _lights) 
                    lightComponent.shadows = LightShadows.None;
            }
        }
    }
}