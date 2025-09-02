using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Effects.Model;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects
{
    public class ParticleEffect : VisualEffect
    {
        [Serializable]
        private class EffectConfig
        {
            public string Key;
            public Vector3 Scale = Vector3.one;
        }
        
        [SerializeField] private EffectConfig[] _configs;
        [SerializeField] private List<ParticleSystem> _particles;
        
        [Inject] private EffectFactory _effectFactory;
        
        public override void Activate(UnitView unitView)
        {
            base.Activate(unitView);
            SpawnEffects(unitView.ParticlePoint.position);
        }
        
        private void SpawnEffects(Vector3 position)
        {
            foreach (var config in _configs)
            {
                var effect = _effectFactory.Create(config.Key);
                effect.transform.localScale = config.Scale;
                effect.transform.position = position;
            }

            foreach (var particle in _particles) 
                particle.Play();
        }
    }
}