using System;
using System.Collections.Generic;
using System.Linq;
using Animancer.FSM;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Building;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates
{
    [HideMonoScript]
    public abstract class VisualState<T> : StateBehaviour, IBuildElement where T : Enum
    {
        [SerializeField] private UnitView _unit;
        [SerializeField] private List<VisualEffect> _effects;
        
        [ShowInInspector] public virtual StatePriority Priority => StatePriority.Low;
        [ShowInInspector] public abstract T Type { get; }

        protected UnitView Unit => _unit;
        private StateMachine<VisualState<T>> _stateMachine;

        void IBuildElement.LogIfWrong(ref int errorCount)
        {
            if (_unit == null)
            {
                errorCount++;
                Logger.LogError($"No unit provided on {gameObject.name}", gameObject);
            }

            if (_effects.Count == 0) 
                Logger.LogWarning($"Effects are not set on {gameObject.name}", gameObject);
            if (_effects.Any(item => item == null))
            {
                errorCount++;
                Logger.LogError($"Effects are not set on {gameObject.name}", gameObject);
            }
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (Application.isPlaying) return;
            _unit ??= gameObject.GetComponentInParent<UnitView>();
        }
#endif

        [Button]
        private void FillEffects()
        {
            _effects.Clear();
            _effects.AddRange(GetComponentsInChildren<VisualEffect>());
        }

        public void SetupEffects(IReadOnlyList<VisualEffect> effects)
        {
            _effects.Clear();
            foreach (var effect in effects) 
                _effects.Add(effect);
        } 

        public void Initialize(StateMachine<VisualState<T>> stateMachine) => 
            _stateMachine = stateMachine;

        public override bool CanEnterState => true;

        public override bool CanExitState
        {
            get
            {
                VisualState<T> nextState = _stateMachine.NextState;
                if (nextState == this)
                    return CanInterruptSelf;
                return nextState.Priority >= Priority;
            }
        }

        protected virtual bool CanInterruptSelf => true;

        public override void OnEnterState()
        {
            base.OnEnterState();

            foreach (var effect in _effects) 
                effect.Activate(_unit);
        }

        public override void OnExitState()
        {
            base.OnExitState();
            
            foreach (var effect in _effects) 
                effect.Deactivate(_unit);
        }
    }
}