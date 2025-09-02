using System;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Type;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Model
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PopupResource : MonoBehaviour
    {
        [Serializable]
        private class SecondParticleConfig
        {
            public PopupIconEffect Type;
            public ParticleImage MainParticle;
            public List<ParticleImage> AllParticles;

            public void Initialize()
            {
                MainParticle.onLastParticleFinished.AddListener(DisableAllParticles);
                DisableAllParticles();
            }

            public void DisableAllParticles()
            {
                foreach (var particle in AllParticles) 
                    particle.enabled = false;
            }
            
            public void EnableAllParticles()
            {
                foreach (var particle in AllParticles) 
                    particle.enabled = true;
            }
        }

        [SerializeField] private ParticleImage _mainParticle;
        [SerializeField] private List<SecondParticleConfig> _secondParticles;
        
        public ParticleImage MainParticle => _mainParticle;

        private void Awake()
        {
            foreach (var secondParticle in _secondParticles)
                secondParticle.Initialize(); 
        }

        private void Reset() =>
            _mainParticle = GetComponentInChildren<ParticleImage>();

        public void PlaySecondParticles(PopupIconEffect type)
        {
            foreach (var secondParticle in _secondParticles)
            {
                if (!type.HasFlag(secondParticle.Type)) continue;
                secondParticle.EnableAllParticles();
                secondParticle.MainParticle.Play();
                break;
            }
        }
    }
}