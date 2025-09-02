using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.TweenUtilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Animations
{
    public class UpdateObjectsRotation : BaseAnimation
    {
        [SerializeField, Required] private List<Transform> _targets;
        [SerializeField, Required] private RotationAxis _axis = RotationAxis.Y;
        [SerializeField, MinValue(0)] private float _rotationSpeed;
        [SerializeField, MinValue(0)] private float _smoothTime = 0.5f;

        private Vector3 _rotationDelta;
        private float _currentSpeed;
        private float _velocity;
        
        public bool IsPlaying { get; private set; }

        private void Awake() => 
            _rotationDelta = GetRotationVector();

        protected override void OnDisable()
        {
            base.OnDisable();
            _currentSpeed = 0;
        }

        public override void Play()
        {
            IsPlaying = true;
        }

        public override void Stop()
        {
            IsPlaying = false;
        }

        private void Update()
        {
            float targetSpeed = IsPlaying ? _rotationSpeed : 0f;
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _velocity, _smoothTime);
            
            var rotationAmount = _rotationDelta.normalized * (_currentSpeed * Time.deltaTime);
            
            foreach (var target in _targets) 
                target.Rotate(rotationAmount, Space.Self);
        }

        private Vector3 GetRotationVector()
        {
            return new Vector3()
            {
                x = _axis == RotationAxis.X ? _rotationSpeed : 0,
                y = _axis == RotationAxis.Y ? _rotationSpeed : 0,
                z = _axis == RotationAxis.Z ? _rotationSpeed : 0
            };
        }
    }
}