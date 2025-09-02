using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace PleasantlyGames.RPG.Runtime.Core.Location.View.Component.Movement
{
    [DisallowMultipleComponent, HideMonoScript]
    public class LocationMovement : MonoBehaviour
    {
        [Serializable]
        private class PlatformConfig : IWeightedRandom
        {
            public LocationPlatform PlatformTemplate;
            [MinValue(0)] public int Weight;
            public int RandomWeight => Weight;
            
            private ObjectPool<LocationPlatform> _objectPool;

            public void Initialize() => 
                _objectPool = new ObjectPool<LocationPlatform>(SpawnNewPlatform, defaultCapacity: 2);

            public LocationPlatform Get() => 
                _objectPool.Get();

            public void Release(LocationPlatform locationPlatform)
            {
                locationPlatform.Clear();
                _objectPool.Release(locationPlatform);
            }

            private LocationPlatform SpawnNewPlatform() => 
                Instantiate(PlatformTemplate);
        }

        [SerializeField] private List<PlatformConfig> _platformConfigs;
        [SerializeField] private Transform _parent;
        [SerializeField] private int _activePlatformAmount = 2;
        [SerializeField, MinValue(0)] private float _platformLength;
        [SerializeField, MinValue(0)] private float _timeToStop = 0.35f;

        [SerializeField] private UnityEvent _onStartMovement;
        [SerializeField] private UnityEvent _onStopMovement;
        
        [ShowInInspector, HideInEditorMode, ReadOnly] private readonly List<LocationPlatform> _currentPlatforms = new();
        [ShowInInspector, HideInEditorMode] private float _speed;
        [ShowInInspector, HideInEditorMode, ReadOnly] private bool _isMoving;
        private readonly Dictionary<LocationPlatform, PlatformConfig> _platformDictionary = new();
        
        private float _halfPlatformLength;
        private float _endBorder;
        private Tween _stopMovement;

        public float TimeToStop => _timeToStop;
        public float Speed => _speed;
        public UnityEvent OnStartMovement => _onStartMovement;
        public UnityEvent OnStopMovement => _onStopMovement;

        private void Reset() => _parent = transform;

        private void Awake() => Initialization();

        private void Initialization()
        {
            foreach (var platform in _platformConfigs) 
                platform.Initialize();
            
            _halfPlatformLength = _platformLength / 2;
            var totalLength = _activePlatformAmount * _platformLength;
            var halfTotalLength = totalLength / 2;
            _endBorder = -halfTotalLength;
            
            for (int i = 0; i < _activePlatformAmount; i++)
            {
                var platform = SpawnPlatform();
                platform.SetLocalPosition(new Vector3((i * _platformLength + _platformLength / 2) - halfTotalLength, 0, 0));
            }
        }

        [Button]
        public void StartMovement(float speed)
        {
            _stopMovement.Stop();
            _speed = speed;
            _isMoving = true;
            _onStartMovement?.Invoke();
        }

        [Button]
        public void StopMovement()
        {
            _stopMovement.Stop();
            _stopMovement = Tween.Custom(_speed, 0, _timeToStop, (value) => _speed = value)
                .OnComplete(() => _isMoving = false);
            
            _onStopMovement?.Invoke();
        }

        public void Update()
        {
            if(!_isMoving) return;
            
            var moveDelta = Vector3.left * (_speed * Time.deltaTime);
            foreach (var platform in _currentPlatforms) 
                platform.AddLocalPosition(moveDelta);

            var firstPlatform = _currentPlatforms[0];
            if (firstPlatform.transform.localPosition.x - _halfPlatformLength < _endBorder)
            {
                var platformConfig = _platformDictionary[firstPlatform];
                platformConfig.Release(firstPlatform);
                _platformDictionary.Remove(firstPlatform);
                _currentPlatforms.RemoveAt(0);
                
                var lastPlatform = _currentPlatforms[^1];
                var newPlatform = SpawnPlatform();
                newPlatform.SetLocalPosition(lastPlatform.transform.position + Vector3.right * _platformLength);
            }
        }

        private LocationPlatform SpawnPlatform()
        {
            var platformConfig = _platformConfigs.WeightedRandomElement();
            var platform = platformConfig.Get();
            _platformDictionary.Add(platform, platformConfig);
            _currentPlatforms.Add(platform);
            platform.transform.SetParent(_parent);
            
            return platform;
        }

        public LocationPlatform GetClosestPlatform(Vector3 targetPosition)
        {
            float fakeDistance = float.MaxValue;
            LocationPlatform result = null;
            
            foreach (var platform in _currentPlatforms)
            {
                var distance = Vector3.SqrMagnitude(platform.transform.position - targetPosition);
                if (distance < fakeDistance)
                {
                    fakeDistance = distance;
                    result = platform;
                }
            }

            return result;
        }

        public void Clear()
        {
            foreach (var locationPlatform in _currentPlatforms) 
                locationPlatform.Clear();
        }
    }
}