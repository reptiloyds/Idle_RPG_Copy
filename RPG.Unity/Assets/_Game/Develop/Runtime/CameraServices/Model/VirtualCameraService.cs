using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using PleasantlyGames.RPG.Runtime.CameraServices.Contract;
using PleasantlyGames.RPG.Runtime.CameraServices.Defenition;
using PleasantlyGames.RPG.Runtime.CameraServices.Model.Mediator;
using PleasantlyGames.RPG.Runtime.DebugUtilities;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using CameraType = PleasantlyGames.RPG.Runtime.CameraServices.Type.CameraType;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Model
{
    [HideMonoScript, DisallowMultipleComponent]
    internal class VirtualCameraService : MonoBehaviour, IVirtualCameraService
    {
        [SerializeField] private int _mainPriority = 10;
        [SerializeField] private List<CinemachineVirtualCameraBase> _virtualCameraBases;
        [SerializeField] private VirtualCameraConfig[] _virtualCameras;

        private VirtualCameraConfig _cameraConfig;
        private CinemachineBrain _brain;
        
        [Inject] private CameraConfiguration _configuration;
        [Inject] private ICameraProvider _cameraProvider;

        public event Action OnCameraChanged;

        private void Reset() => 
            GetComponents();

        private void OnValidate() => 
            GetComponents();

        [Button]
        private void GetComponents() => 
            _virtualCameraBases = GetComponentsInChildren<CinemachineVirtualCameraBase>().ToList();

        private void Awake()
        {
            foreach (var virtualCameraBase in _virtualCameraBases) 
                virtualCameraBase.Priority = _mainPriority - 1;
            
            foreach (var virtualCamera in _virtualCameras)
            {
                if (virtualCamera.Type == CameraType.Main)
                {
                    virtualCamera.Interlayer.Instance.m_Priority = _mainPriority;
                    _cameraConfig = virtualCamera;   
                }
                
                virtualCamera.DefaultPriority = virtualCamera.Interlayer.Instance.m_Priority;
            }
            
            _brain = _cameraProvider.GetBrain();
        }

        CameraType IVirtualCameraService.GetCurrentCameraType() => 
            _cameraConfig.Type;

        CinemachineInterlayer IVirtualCameraService.GetCurrentCamera() => 
            _cameraConfig.Interlayer;

        CinemachineInterlayer IVirtualCameraService.GetCamera(CameraType type)
        {
            foreach (var virtualCamera in _virtualCameras)
                if(virtualCamera.Type == type)
                    return virtualCamera.Interlayer;

            return null;
        }

        void IVirtualCameraService.SwitchTo(CameraType type, float blendTime = -1f)
        {
            VirtualCameraConfig cameraConfig = GetCameraConfigByType(type);

            if (cameraConfig == null)
            {
                Logger.LogError($"Can`t find {typeof(VirtualCameraConfig)} type of {type}");
                return;
            }
            
            SwitchTo(cameraConfig, blendTime);
        }

        public void SwitchTo(VirtualCameraConfig cameraConfig, float blendTime = -1f)
        {
            SetBlendTime(blendTime);
            ResetPriority();
            var maxPriority = GetMaxPriority();
            
            _cameraConfig = cameraConfig;
            _cameraConfig.Interlayer.Instance.m_Priority = maxPriority + 1;
            
            OnCameraChanged?.Invoke();
        }

        private void ResetPriority()
        {
            if (_cameraConfig != null) 
                _cameraConfig.ResetPriority();
            
            foreach (var virtualCamera in _virtualCameras)
                virtualCamera.ResetPriority();
        }

        private int GetMaxPriority()
        {
            int maxPriority = 0;
            foreach (var virtualCamera in _virtualCameras)
            {
                if (virtualCamera.Interlayer.Instance.m_Priority > maxPriority) 
                    maxPriority = virtualCamera.Interlayer.Instance.m_Priority;
            }

            return maxPriority;
        }

        private VirtualCameraConfig GetCameraConfigByType(CameraType type)
        {
            foreach (var virtualCamera in _virtualCameras)
            {
                if (virtualCamera.Type == type) 
                    return virtualCamera;
            }

            return null;
        }

        private void SetBlendTime(float blendTime)
        {
            if(blendTime < 0)
                blendTime = _configuration.BlendTime;
            _brain.m_DefaultBlend.m_Time = blendTime;
        }
    }
}
