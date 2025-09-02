using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat
{
    [DisallowMultipleComponent, HideMonoScript]
    public class EnemyTargets : UnitComponent
    {
        [Serializable]
        private class CombatTarget
        {
            public Transform Point;
            public GameObject Enemy;
        }
        
        [SerializeField] private float _radius = 1f;
        [SerializeField] private int _numberOfTargets = 5;
        [SerializeField] private float _angelOffset = 90;
        [SerializeField] private List<CombatTarget> _targets = new();

        private bool _rotationIsAnchored;
        private Quaternion _worldRotationAnchor;

        public override void OnSpawn()
        {
            base.OnSpawn();
            ClearTargets();
        }

        public override void Dispose()
        {
            base.Dispose();
            ClearTargets();
        }

        private void ClearTargets()
        {
            foreach (var target in _targets) 
                target.Enemy = null;
        }

        public Transform GetTarget(GameObject key)
        {
            if (_targets.Count == 0) return null;

            foreach (var target in _targets)
            {
                if (target.Enemy == key)
                    return target.Point;
            }
            
            CombatTarget result = null;
            float minDistance = float.MaxValue;
            foreach (var target in _targets)
            {
                if (target.Enemy != null) continue;
                var fakeDistance = Vector3.Magnitude(target.Point.position - key.transform.position);
                if (fakeDistance < minDistance)
                {
                    minDistance = fakeDistance;
                    result = target;
                }
            }

            if(result == null) return null;
            
            result.Enemy = key;
            return result.Point;
        }

        public void ReleasePoint(GameObject key)
        {
            foreach (var target in _targets)
            {
                if (target.Enemy != key) continue;
                target.Enemy = null;
                break;
            }
        }

        public void FixPoints()
        {
            _rotationIsAnchored = true;
            transform.localRotation = Quaternion.identity;
            _worldRotationAnchor = transform.rotation;
        }

        private void LateUpdate()
        {
            if (_rotationIsAnchored) 
                transform.rotation = _worldRotationAnchor;
        }

        [Button()]
        public void SpawnTargets()
        {
            ClearOldTargets();


            for (int i = 0; i < _numberOfTargets; i++)
            {
                float angleInDegrees = i * 360f / _numberOfTargets + _angelOffset;
                float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

                Vector3 offset = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians)) * _radius;
                var spawnPosition = transform.position + offset;
                var target = new GameObject($"EnemyMoveTarget_{i}")
                {
                    transform =
                    {
                        position = spawnPosition,
                        parent = transform
                    }
                };
                _targets.Add(new CombatTarget()
                {
                    Point = target.transform,
                });
            }
        }

        [Button]
        private void ClearOldTargets()
        {
            for (var i = _targets.Count - 1; i >= 0; i--)
            {
                var target = _targets[i];
                DestroyImmediate(target.Point);
            }
            _targets.Clear();
        }

        private void OnDrawGizmos()
        {
            foreach (var target in _targets)
            {
                Gizmos.color = target.Enemy == null ? Color.green : Color.red;
                Gizmos.DrawSphere(target.Point.position, 0.15f);
            } 
            Gizmos.color = Color.white;
        }
    }
}