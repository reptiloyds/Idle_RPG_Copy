using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Projectile.Model.Projectile.State;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Projectile.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public sealed class BaseProjectile : MonoBehaviour
    {
        [SerializeField] private bool _blockMultipleCollisions = true;
        [SerializeField, Required] private Transform _visualParent;
        [SerializeField, MinValue(0)] private float _defaultMaxDistance = 25f;
        [SerializeField] private float _collisionRadius = 0.3f;
        [SerializeField] private LayerMask _layerMask;

        [ShowInInspector]
        private readonly Collider[] _colliderBuffer = new Collider[3];
        
        [ShowInInspector, HideInEditorMode]
        private float _speed;
        [ShowInInspector, HideInEditorMode]
        private float _maxDistance;
        [ShowInInspector, HideInEditorMode, ReadOnly]
        private float _distance;
        private readonly HashSet<Collider> _colliders = new();
        private bool _isInitialized;
        private bool _isStateEmpty = true;
        [ShowInInspector, HideInEditorMode]
        private ProjectileState _state;
        private ProjectileFollowTransform _followTransform;
        private ProjectileFollowDirection _followDirection;
        
        public Vector3 StartPosition { get; private set; }
        public Vector3 TotalDirection => transform.position - StartPosition;
        public float LeftDistance => _maxDistance - _distance;
        public string VisualId { get; private set; } = default;
        public GameObject Visual { get; private set; }
        
        public event Action<BaseProjectile, Collider> TriggerEnter;
        public event Action<BaseProjectile> Complete;

        [Button]
        public void Initialize()
        {
            if(_isInitialized) return;
            _isInitialized = true;
            _followTransform = new ProjectileFollowTransform(this);
            _followDirection = new ProjectileFollowDirection(this);
            _followTransform.Complete += OnComplete;
            _followDirection.Complete += OnComplete;
        }

        private void OnDestroy()
        {
            if(_followTransform != null)
                _followTransform.Complete -= OnComplete;
            if(_followDirection != null)
                _followDirection.Complete -= OnComplete;
        }

        private void OnDisable()
        {
            SetState(null);
            _colliders.Clear();
            Array.Clear(_colliderBuffer, 0, _colliderBuffer.Length);
        }

        public void SetVisual(GameObject visual, string visualId)
        {
            VisualId = visualId;
            Visual = visual;
            Visual.transform.SetParent(_visualParent);
            Visual.transform.localPosition = Vector3.zero;
            Visual.transform.localRotation = Quaternion.identity;
        }

        public void ClearVisual()
        {
            VisualId = default;
            Visual = null;
        }

        public void Setup(float speed, float maxDistance = -1)
        {
            _speed = speed;
            _maxDistance = maxDistance > 0 ? maxDistance : _defaultMaxDistance;
            _distance = 0;
        }
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void MoveTo(Transform target, Vector3 offset)
        {
            _followTransform.Setup(target, offset);
            SetState(_followTransform);

            StartPosition = transform.position;
        }

        [Button]
        public void MoveByDirection(Vector3 targetDirection)
        {
            _followDirection.Setup(targetDirection);
            SetState(_followDirection);
            
            StartPosition = transform.position;
        }
        
        public void Move(Vector3 direction)
        {
            var delta = _speed * Time.deltaTime;
            transform.position += direction.normalized * delta;
            _distance += delta;
            
            if (_distance > _maxDistance)
                OnComplete();
        }

        public void MovePosition(Vector3 position)
        {
            var distance = Vector3.Distance(transform.position, position);
            transform.position = position;
            _distance += distance;
            
            if (_distance > _maxDistance)
                OnComplete();
        }

        public void Forward(Vector3 direction)
        {
            if(direction == Vector3.zero) return;
            transform.forward = direction; 
        }

        private void Update()
        {
            CheckCollisions();
            if(_isStateEmpty) return;
            _state.Update();
        }

        private void CheckCollisions()
        {
            var hits = Physics.OverlapSphereNonAlloc(transform.position, _collisionRadius, _colliderBuffer, _layerMask);
        
            for (var i = 0; i < hits; i++)
            {
                Collider hit = _colliderBuffer[i];
                if(hit == null) continue;
                OnTriggerEnter(hit);
                return;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_blockMultipleCollisions && !_colliders.Add(other)) return;
            TriggerEnter?.Invoke(this, other);
        }

        private void OnComplete()
        {
            SetState(null);
            Complete?.Invoke(this);
        }

        private void SetState(ProjectileState state)
        {
            _state?.Exit();
            _state = state;
            _isStateEmpty = _state == null;
            _state?.Enter();
        }
    }
}
