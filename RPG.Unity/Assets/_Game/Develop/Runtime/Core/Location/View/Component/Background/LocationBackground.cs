using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Location.View.Component.Movement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Location.View.Component.Background
{
    [DisallowMultipleComponent, HideMonoScript]
    public class LocationBackground : MonoBehaviour
    {
        [SerializeField] private LocationMovement _movement;
        [SerializeField] private List<BaseParallax> _parallaxEffects;

        private void Reset() => 
            GetComponents();

        private void OnValidate() => 
            GetComponents();

        private void GetComponents()
        {
            _movement ??= GetComponentInParent<LocationMovement>();
            _parallaxEffects ??= GetComponentsInChildren<BaseParallax>().ToList();
        }

        [Button]
        private void UpdateParallaxes() => 
            _parallaxEffects = GetComponentsInChildren<BaseParallax>().ToList();

        [Button]
        private void Build()
        {
            Clear();
            foreach (var parallaxEffect in _parallaxEffects) 
                parallaxEffect.Build();
        }

        [Button]
        private void Clear()
        {
            foreach (var parallaxEffect in _parallaxEffects) 
                parallaxEffect.Clear();
        }

        private void Awake()
        {
            _movement.OnStartMovement.AddListener(Run);
            _movement.OnStopMovement.AddListener(Stop);
        }

        private void OnDestroy()
        {
            _movement.OnStartMovement.RemoveListener(Run);
            _movement.OnStopMovement.RemoveListener(Stop);
        }

        private void Run()
        {
            foreach (var parallaxEffect in _parallaxEffects) 
                parallaxEffect.Run(_movement.Speed);
        }

        private void Stop()
        {
            foreach (var parallaxEffect in _parallaxEffects) 
                parallaxEffect.Stop(_movement.TimeToStop);
        }
    }
}
