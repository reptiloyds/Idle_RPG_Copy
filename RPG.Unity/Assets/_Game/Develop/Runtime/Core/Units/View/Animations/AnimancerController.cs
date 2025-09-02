using System;
using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Animations
{
    public class AnimancerController : UnitComponent
    {
        [Serializable]
        private class LayerConfigs
        {
            public AnimancerLayerType Type;
            public bool ResetOnSpawn;
            [HideIf("@this.ResetOnSpawn == false")]
            public float DefaultWeight;
        }
        
        [SerializeField] private AnimancerComponent _component;

        [SerializeField] private List<LayerConfigs> _configs = new()
        {
            new LayerConfigs {Type = AnimancerLayerType.Movement, ResetOnSpawn = true, DefaultWeight = 1f},
            new LayerConfigs {Type = AnimancerLayerType.Combat, ResetOnSpawn = true, DefaultWeight = 0f},
            new LayerConfigs {Type = AnimancerLayerType.Damage, ResetOnSpawn = true, DefaultWeight = 0f},
        };
        [SerializeField] private bool _combatLayerIsAdditive = false;
        [SerializeField] private AvatarMask _combatMask;
        [SerializeField] private bool _damageLayerIsAdditive = false;
        [SerializeField] private AvatarMask _damageMask;
        [SerializeField] private StringAsset _attackEvent;
        
        private readonly Dictionary<AnimancerLayerType, AnimancerLayer> _layers = new();

        protected override void GetComponents()
        {
            base.GetComponents();
            _component ??= GetComponent<AnimancerComponent>();
        }

        public override void Initialize()
        {
            base.Initialize();
            
            var movementLayer = _component.Layers[0];
            movementLayer.SetDebugName("MovementLayer");
            _layers.Add(AnimancerLayerType.Movement, movementLayer);
           
            var damageLayer = _component.Layers[1];
            damageLayer.SetDebugName("DamageLayer");
            damageLayer.IsAdditive = _damageLayerIsAdditive;
            damageLayer.Mask = _damageMask;
            _layers.Add(AnimancerLayerType.Damage, damageLayer);

            var combatLayer = _component.Layers[2];
            combatLayer.SetDebugName("CombatLayer");
            combatLayer.IsAdditive = _combatLayerIsAdditive;
            combatLayer.Mask = _combatMask;
            _layers.Add(AnimancerLayerType.Combat, combatLayer);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            foreach (var config in _configs)
            {
                if(!config.ResetOnSpawn) continue;
                var layer = GetLayer(config.Type);
                layer.Weight = config.DefaultWeight;
            }
            
            _component.Events.AddNew(_attackEvent, OnAttackEvent);
        }

        public override void Dispose()
        {
            base.Dispose();
            _component.Events.Remove(_attackEvent);
        }

        public void InitializeState(AnimancerLayerType layerType, ClipTransition clip, float weight = -1)
        {
            var layer = GetLayer(layerType);
            var state = layer.GetOrCreateState(clip);
            if (weight >= 0) 
                state.Weight = weight;
        }

        public AnimancerState PlayOnLayer(AnimancerLayerType layerType, ClipTransition clip)
        {
            var layer = GetLayer(layerType);
            return layer.Play(clip);
        }

        public void FadeLayer(AnimancerLayerType layerType, float weight, float duration)
        {
            var layer = GetLayer(layerType);
            if (weight == 0) weight = 0.001f;
            layer.StartFade(weight, duration);
            foreach (var state in layer)
            {
                if (state.IsPlaying || state.Weight < 0.9f) continue;
                state.Play();
                break;
            }
        }

        private AnimancerLayer GetLayer(AnimancerLayerType type) => 
            _layers[type];

        private void OnAttackEvent() => 
            Unit.Combat.TriggerAttack();
    }
}