using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Camera
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CameraShaker : MonoBehaviour, IInitializable
    {
        [SerializeField, Required] private CinemachineVirtualCamera _virtualCamera;

        private CinemachineBasicMultiChannelPerlin _multiChannel;
        private float _shakeTime;
        private bool _isActive;

        void IInitializable.Initialize() => 
            _multiChannel = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        public void Shake(float amplitude, float time)
        {
            _isActive = true;
            _multiChannel.m_AmplitudeGain = amplitude;
            _shakeTime = time;
        }

        public void Stop()
        {
            _isActive = false;
            _multiChannel.m_AmplitudeGain = 0;
            _shakeTime = 0;
        }

        public void Update()
        {
            if (!_isActive) return;

            _shakeTime -= Time.deltaTime;
            if (_shakeTime <= 0) Stop();
        }
    }
}