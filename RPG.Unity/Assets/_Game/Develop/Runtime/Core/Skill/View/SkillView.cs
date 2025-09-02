using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Skill.View.Component.Base;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private List<SkillViewComponent> _components;
        [SerializeField] private bool _followInstantly = true;
        [SerializeField, HideIf("@this._followInstantly == true")]
        private float _followSpeed = 10f;
        [SerializeField, HideIf("@this._followInstantly == true")]
        private float _stopFollowDistance = 0.1f;
        [ShowInInspector, ReadOnly, HideInEditorMode] private string _id;

        [ReadOnly, ShowIf("@this._hasFollowTarget == true")]
        private Transform _followTarget;
        private bool _hasFollowTarget;
        private Vector3 _offset;

        private Tween _despawnTween;

        public event Action<SkillView> OnDespawn;
        public string Id => _id;

        private void Reset() => 
            GetComponents();

        private void OnValidate() => 
            GetComponents();

        [Button]
        private void GetComponents() => 
            _components = GetComponentsInChildren<SkillViewComponent>().ToList();

        public void Spawn(Vector3 position, Vector3 scale, string id)
        {
            _id = id;
            transform.position = position;
            transform.localScale = scale;

            foreach (var component in _components) 
                component.OnSpawn();
        }

        public void ForceDespawn()
        {
            _despawnTween.Stop();
            
            foreach (var component in _components) 
                component.OnDespawn();
            
            CompleteDespawn();
        }
        
        public void SmoothDespawn(float delay = 0)
        {
            _despawnTween.Stop();
            
            if (delay <= 0) 
                StartDespawn();
            else
                _despawnTween = Tween.Delay(delay, StartDespawn);
        }

        private void StartDespawn()
        {
            float maxDespawnTime = 0;
            foreach (var component in _components)
            {
                var despawnTime = component.GetDespawnTime();
                if (despawnTime > maxDespawnTime) 
                    maxDespawnTime = despawnTime;
                component.OnDespawn();
            }

            if (maxDespawnTime > 0) 
                Tween.Delay(maxDespawnTime, CompleteDespawn);
            else
                CompleteDespawn();
        }

        public void Follow(Transform target, Vector3 offset = default)
        {
            _hasFollowTarget = true;
            _followTarget = target;
            _offset = offset;
        }

        public void StopFollow()
        {
            _hasFollowTarget = false;
            _followTarget = null;
        }

        private void Update()
        {
            if (!_hasFollowTarget) return;
            if (_followInstantly)
                MoveInstantly();
            else
                MoveSmooth();
        }

        private void MoveInstantly()
        {
            transform.position = _followTarget.position + _offset;
        }

        private void MoveSmooth()
        {
            var realPosition = _followTarget.position + _offset;
            if (Vector3.Distance(transform.position, realPosition) <= _stopFollowDistance) return;
            var direction = _followTarget.position - transform.position;
            transform.position += direction * (_followSpeed * Time.deltaTime);
        }

        private void CompleteDespawn()
        {
            if(_hasFollowTarget)
                StopFollow();
            OnDespawn?.Invoke(this);
        }
    }
}