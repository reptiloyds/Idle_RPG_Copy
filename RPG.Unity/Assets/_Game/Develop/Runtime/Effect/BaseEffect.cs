using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Effect
{
    [HideMonoScript, DisallowMultipleComponent]
    public class BaseEffect : MonoBehaviour
    {
        [SerializeField] private Vector3 _spawnOffset;
        [SerializeField] private bool _setupLifetime;
        [SerializeField, MinValue(0), HideIf("@this._setupLifetime == false")] private float _lifeTime;
        [ShowInInspector, ReadOnly, HideInEditorMode] private Transform _followTarget;
        [ShowInInspector, ReadOnly, HideInEditorMode] private Vector3 _offset;
        [ShowInInspector, ReadOnly, HideInEditorMode] private string _id;

        private bool _isFollowing;
        private bool _hasLifeTime;
        private float _timer;

        public string Id => _id;
        public event Action<BaseEffect> OnRelease;

        private void OnEnable()
        {
            if(_setupLifetime)
                SetLifetime(_lifeTime);
        }

        public void SetId(string id) => 
            _id = id;

        public void SetLifetime(float lifeTime)
        {
            _hasLifeTime = true;
            _timer = lifeTime;
        }

        public void SetPosition(Vector3 position) => 
            transform.position = position + _spawnOffset + _offset;


        public void Follow(Transform target, Vector3 offset = default)
        {
            _isFollowing = true;
            _followTarget = target;
            _offset = offset;
            SetPosition(_followTarget.position);
        }

        public void Release() => 
            OnRelease?.Invoke(this);

        private void Update()
        {
            if (_hasLifeTime)
            {
                _timer -= Time.deltaTime;
                if (_timer > 0) return;
                Release();
                _hasLifeTime = false;   
            }

            if (_isFollowing)
            {
                SetPosition(_followTarget.position);   
            }
        }

        private void OnDisable()
        {
            _isFollowing = false;
            _followTarget = null;
            _hasLifeTime = false;
        }
    }
}