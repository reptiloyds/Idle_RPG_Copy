using System;
using Cinemachine;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Camera
{
    public class CameraFOVAdapter : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField, Required] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField, Required] private CameraPositioner _cameraPositioner;
        [SerializeField, MinValue(20)] private float _minFOV = 20f;
        [SerializeField, MinValue(20)] private float _maxFOV = 90f;
        [SerializeField, MinValue(0)] private float _angelPadding = 5f;
        [Inject] private LocationFactory _locationFactory;
        
        public void Initialize()
        {
            if(!enabled) return;
            
            _locationFactory.OnLocationLoaded += RefreshFOV;
            _cameraPositioner.OnComplete += RefreshFOV;
            if (_locationFactory.Location != null) 
                RefreshFOV();
        }

        public void Dispose()
        {
            _locationFactory.OnLocationLoaded -= RefreshFOV;
            _cameraPositioner.OnComplete -= RefreshFOV;
        }

        [Button]
        private void RefreshFOV()
        {
            var allyPoints = _locationFactory.Location.AllySpawn.GetCurrentPoints();
            var companionPoints = _locationFactory.Location.CompanionSpawn.GetCurrentPoints();

            Vector3 borderPosition = new Vector3(float.MaxValue, 0, 0);
            
            foreach (var spawnPoint in allyPoints)
            {
                if (spawnPoint.Point.position.x < borderPosition.x) 
                    borderPosition = spawnPoint.Point.position;
            }
            foreach (var spawnPoint in companionPoints)
            {
                if (spawnPoint.Point.position.x < borderPosition.x) 
                    borderPosition = spawnPoint.Point.position;
            }
            
            AdjustFOVWithPadding(borderPosition, _angelPadding);
        }
        
        private void AdjustFOVWithPadding(Vector3 worldPosition, float padding)
        {
            Vector3 localPosition = _virtualCamera.transform.InverseTransformPoint(worldPosition);
            
            float horizontalAngle = Mathf.Atan2(localPosition.x, localPosition.z) * Mathf.Rad2Deg;
            float verticalAngle = Mathf.Atan2(localPosition.y, localPosition.z) * Mathf.Rad2Deg;
            
            horizontalAngle = Mathf.Abs(horizontalAngle) + padding;
            verticalAngle = Mathf.Abs(verticalAngle) + padding;
            
            float requiredFOV = Mathf.Max(horizontalAngle * 2, verticalAngle * 2);
            
            _virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(requiredFOV, _minFOV, _maxFOV);
        }
    }
}