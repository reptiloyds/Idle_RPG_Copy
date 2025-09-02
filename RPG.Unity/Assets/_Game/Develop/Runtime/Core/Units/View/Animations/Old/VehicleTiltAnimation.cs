using PleasantlyGames.RPG.Runtime.TweenUtilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Animations.Old
{
    [DisallowMultipleComponent, HideMonoScript]
    public class VehicleTiltAnimation : BaseAnimation
    {
        [SerializeField, Required] private Transform _body;
        [SerializeField] private float _tiltAmount = 7.5f;
        [SerializeField] private float _tiltSpeed = 2f;
        [SerializeField] private float _simulationSpeed = 5f;
        [SerializeField] private float _accelerationThreshold = 0.1f;
        [SerializeField] private float _startAccelerationSpeed = 4f;
        [SerializeField] private float _stopAccelerationSpeed = 10f;
        
        private float _previousSpeed;
        private float _currentSpeed;
        
        public override void Play() => 
            _currentSpeed = _simulationSpeed;

        public override void Stop() => 
            _currentSpeed = 0;

        private void Update() => TiltBody();

        private void TiltBody()
        {
            var speedDelta = _currentSpeed - _previousSpeed;
            
            var targetTilt = 0f;
            if (Mathf.Abs(speedDelta) > _accelerationThreshold)
            {
                if (speedDelta > 0)
                {
                    targetTilt = -_tiltAmount;
                    _previousSpeed = Mathf.Lerp(_previousSpeed, _currentSpeed, Time.deltaTime * _startAccelerationSpeed);
                }
                else if (speedDelta < 0)
                {
                    targetTilt = _tiltAmount;
                    _previousSpeed = Mathf.Lerp(_previousSpeed, _currentSpeed, Time.deltaTime * _stopAccelerationSpeed);
                }
            }
            
            var targetRotation = Quaternion.Euler(targetTilt, 0, 0);
            _body.localRotation = Quaternion.Slerp(_body.localRotation, targetRotation, Time.deltaTime * _tiltSpeed);
        }
    }
}