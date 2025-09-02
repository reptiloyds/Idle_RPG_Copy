using System;
using System.Collections.Generic;
using Cinemachine;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Location.View;
using PleasantlyGames.RPG.Runtime.Device;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using DeviceType = PleasantlyGames.RPG.Runtime.Device.DeviceType;

namespace PleasantlyGames.RPG.Runtime.Core.Camera
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CameraPositioner : MonoBehaviour
    {
        [Serializable]
        private class CameraPosition
        {
            public int BranchAmount;
            public Vector3 Position;
            public Vector3 Rotation;
            public float FOV = 45f;
            public TweenSettings<Vector3> StartMoveSettings;
            public TweenSettings<Vector3> StopMoveSettings;
        }
        
        [SerializeField] private Transform _movementContainer;
        
        [SerializeField, Required] private Transform _cameraTransform;
        [SerializeField, Required] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField, MinValue(0)] private float _duration = 1f;
        [SerializeField, MinValue(0)] private Ease _ease = Ease.InOutSine;
        [SerializeField] private List<CameraPosition> _cameraPortPositions;
        [SerializeField] private List<CameraPosition> _cameraLandPositions;
        
        [Inject] private BranchService _branchService;
        [Inject] private LocationFactory _locationFactory;
        [Inject] private IDeviceService _deviceService;

        private List<CameraPosition> _cameraPositions;
        private Tween _movement;
        private LocationView _location;

        private CameraPosition _cameraSetup;
        private Sequence _sequence;

        public event Action OnComplete;
        
        public void Initialize()
        {
            switch (_deviceService.GetOrientation())
            {
                default:
                case OrientationType.Vertical:
                    _cameraPositions = _cameraPortPositions;
                    break;
                case OrientationType.Horizontal:
                    _cameraPositions = _cameraLandPositions;
                    break;
            }
            SetPosition(_branchService.UnlockedBranchesCount());
            _branchService.BranchUnlock += OnBranchUnlock;
            
            _locationFactory.OnLocationLoaded += OnLocationLoaded;
        }

        private void OnDestroy()
        {
            if(_branchService != null)
                _branchService.BranchUnlock -= OnBranchUnlock;
            
            if(_locationFactory != null)
                _locationFactory.OnLocationLoaded -= OnLocationLoaded;
        }

        private void OnLocationLoaded()
        {
            if (_location != null)
            {
                _location.OnMovementStarted -= OnLocationMovementStarted;
                _location.OnMovementStopped -= OnLocationMovementStopped;
            }
            _location = _locationFactory.Location;
            _location.OnMovementStarted += OnLocationMovementStarted;
            _location.OnMovementStopped += OnLocationMovementStopped;
        }

        private void OnLocationMovementStarted()
        {
            _movement.Stop();
            _cameraSetup.StartMoveSettings.startFromCurrent = true;
            _movement = Tween.LocalPosition(_movementContainer, _cameraSetup.StartMoveSettings);
        }

        private void OnLocationMovementStopped()
        {
            _movement.Stop();
            _cameraSetup.StopMoveSettings.startFromCurrent = true;
            _movement = Tween.LocalPosition(_movementContainer, _cameraSetup.StopMoveSettings);
        }

        private void OnBranchUnlock(Branch branch) => 
            MoveToPosition(_branchService.UnlockedBranchesCount());

        [Button, HideInEditorMode]
        private void SetPosition(int branchAmount)
        {
            _cameraSetup = GetCameraSetup(branchAmount);
            _cameraSetup ??= _cameraPositions[^1];
            
            Replace(_cameraSetup);
        }

        [Button, HideInEditorMode]
        public void MoveToPosition(int branchAmount)
        {
            _cameraSetup = GetCameraSetup(branchAmount);
            _cameraSetup ??= _cameraPositions[^1];
            
            _sequence.Stop();

            if (_duration > 0)
            {
                _sequence = Sequence.Create();
                _sequence.Chain(Tween.LocalPosition(_cameraTransform, _cameraSetup.Position, _duration, _ease));
                _sequence.Group(Tween.LocalRotation(_cameraTransform, _cameraSetup.Rotation, _duration, _ease));
                _sequence.Group(Tween.Custom(_virtualCamera.m_Lens.FieldOfView, _cameraSetup.FOV, _duration,
                    (x) => _virtualCamera.m_Lens.FieldOfView = x, _ease));
                _sequence.OnComplete(OnStop);
            }
            else
                Replace(_cameraSetup);
        }

        private void OnStop() => 
            OnComplete?.Invoke();

        private void Replace(CameraPosition cameraSetup)
        {
            _cameraTransform.localPosition = cameraSetup.Position;
            _cameraTransform.localRotation = Quaternion.Euler(cameraSetup.Rotation);
            _virtualCamera.m_Lens.FieldOfView = cameraSetup.FOV;
        }

        private CameraPosition GetCameraSetup(int branchAmount)
        {
            foreach (var cameraPosition in _cameraPositions)
            {
                if (cameraPosition.BranchAmount != branchAmount) continue;
                return cameraPosition;
            }

            return null;
        }
    }
}