using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Behaviour;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Type;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class BaseBrain : UnitComponent
    {
        [ShowInInspector, HideInEditorMode, ReadOnly] private UnitBehaviourType _behaviourType;
        [ShowInInspector, HideInEditorMode, ReadOnly] private string _nodeName;
        
        private readonly Dictionary<UnitBehaviourType, BehaviourMachine> _behaviours = new();
        private BehaviourMachine _behaviour;
        private bool _isActive;
        
        protected void AddBehaviour(UnitBehaviourType type, BehaviourMachine behaviour) => 
            _behaviours.Add(type, behaviour);

        public void SwitchBehaviour(UnitBehaviourType type)
        {
            if (_behaviour != null)
            {
                _behaviour.Exit();   
                _behaviour.OnNodeChanged -= OnNodeChanged;
            }
            
            _behaviour = null;
            _isActive = false;
            
            _behaviourType = type;
            if (!_behaviours.TryGetValue(_behaviourType, out var behaviour)) return;
            
            _behaviour = behaviour;
            _behaviour.OnNodeChanged += OnNodeChanged;
            _behaviour.Enter();
            _isActive = true;
        }

        private void OnNodeChanged() => 
            _nodeName = _behaviour.GetNodeName();

        private void Update()
        {
            if(!_isActive) return;
            _behaviour.Update();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            if (_behaviour != null)
            {
                _behaviour.Exit();
                _behaviour.OnNodeChanged -= OnNodeChanged;
                _behaviour = null;
            }

            _isActive = false;
        }
    }
}