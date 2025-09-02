using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Extensions
{
    [DisallowMultipleComponent, HideMonoScript]
    public class TrailCleaner : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _trailRenderer;

        private bool _cleanOnNextFrame = false;
        private readonly Vector3[] _trailEmptyPositions = Array.Empty<Vector3>();
        
        [Button]
        private void Clear()
        {
            _trailRenderer.Clear();
            _trailRenderer.SetPositions(_trailEmptyPositions);
        }

        private void OnEnable()
        {
            Clear();
            _trailRenderer.enabled = false;
            _cleanOnNextFrame = true;
        }

        private void Update()
        {
            if (_cleanOnNextFrame)
            {
                Clear();
                _trailRenderer.enabled = true;
                _cleanOnNextFrame = false;
            }
        }
    }
}