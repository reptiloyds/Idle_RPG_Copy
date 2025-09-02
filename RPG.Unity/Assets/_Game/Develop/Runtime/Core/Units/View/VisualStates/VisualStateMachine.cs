using System.Collections.Generic;
using System.Linq;
using Animancer.FSM;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates
{
    public class VisualStateMachine : UnitComponent
    {
        [ShowInInspector, ReadOnly, HideInEditorMode] private StateType _state;
        [SerializeField, ReadOnly] private List<VisualEffect> _effects;
        [FoldoutGroup("States"), SerializeField] private StateMachine<VisualState<StateType>> _stateMachine;
        [FoldoutGroup("States"), SerializeField] private VisualState<StateType> _normalState;
        [FoldoutGroup("States"), SerializeField] private VisualState<StateType> _combatState;
        
        [FoldoutGroup("SubStates"), SerializeField] private StateMachine<VisualState<SubStateType>> _movementSubStateMachine;
        [FoldoutGroup("SubStates"), SerializeField] private StateMachine<VisualState<SubStateType>> _attackSubStateMachine;
        [FoldoutGroup("SubStates"), SerializeField] private StateMachine<VisualState<SubStateType>> _damageSubStateMachine;
        [FoldoutGroup("SubStates"), Space]
        [FoldoutGroup("SubStates"), SerializeField] private VisualState<SubStateType> _idleState;
        [FoldoutGroup("SubStates"), SerializeField] private VisualState<SubStateType> _moveState;
        [FoldoutGroup("SubStates"), SerializeField] private VisualState<SubStateType> _attackState;
        [FoldoutGroup("SubStates"), SerializeField] private VisualState<SubStateType> _combatIdleState;
        [FoldoutGroup("SubStates"), SerializeField] private VisualState<SubStateType> _damageState;
        [FoldoutGroup("SubStates"), SerializeField] private VisualState<SubStateType> _deathState;

        protected override void GetComponents()
        {
            base.GetComponents();

            _effects = Unit.gameObject.GetComponentsInChildren<VisualEffect>().ToList();
            
            var states = GetComponentsInChildren<VisualState<StateType>>();
            _normalState ??= states.FirstOrDefault(item => item.Type == StateType.Normal);
            _combatState ??= states.FirstOrDefault(item => item.Type == StateType.Combat);
            
            var subStates = GetComponentsInChildren<VisualState<SubStateType>>();
            _idleState ??= subStates.FirstOrDefault(item => item.Type == SubStateType.Idle);
            _moveState ??= subStates.FirstOrDefault(item => item.Type == SubStateType.Move);
            _attackState ??= subStates.FirstOrDefault(item => item.Type == SubStateType.Attack);
            _combatIdleState ??= subStates.FirstOrDefault(item => item.Type == SubStateType.AttackIdle);
            _damageState ??= subStates.FirstOrDefault(item => item.Type == SubStateType.Damage);
            _deathState ??= subStates.FirstOrDefault(item => item.Type == SubStateType.Death);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            _stateMachine.InitializeAfterDeserialize();
            _movementSubStateMachine.InitializeAfterDeserialize();
            _attackSubStateMachine.InitializeAfterDeserialize();
            _damageSubStateMachine.InitializeAfterDeserialize();
            
            _normalState.Initialize(_stateMachine);
            _combatState.Initialize(_stateMachine);
            
            _idleState.Initialize(_movementSubStateMachine);
            _moveState.Initialize(_movementSubStateMachine);
            _attackState.Initialize(_attackSubStateMachine);
            _combatIdleState.Initialize(_attackSubStateMachine);
            _damageState.Initialize(_damageSubStateMachine);
            _deathState.Initialize(_damageSubStateMachine);   
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            
            _stateMachine.ForceSetState(_normalState);
            _movementSubStateMachine.ForceSetState(_idleState);

            foreach (var effect in _effects) 
                effect.OnSpawn();
        }

        [Button]
        public void SwitchState(StateType type)
        {
            if(_state == type) return;
            _state = type;
            switch (type)
            {
                case StateType.Normal:
                    _stateMachine.TryResetState(_normalState);
                    break;
                case StateType.Combat:
                    _stateMachine.TryResetState(_combatState);
                    break;
            }
        }

        [Button]
        public void SetSubState(SubStateType type)
        {
            switch (type)
            {
                case SubStateType.Idle:
                    _movementSubStateMachine.TryResetState(_idleState);
                    break;
                case SubStateType.Move:
                    _movementSubStateMachine.TryResetState(_moveState);
                    break;
                case SubStateType.AttackIdle:
                    _attackSubStateMachine.TryResetState(_combatIdleState);
                    break;
                case SubStateType.Attack:
                    _attackSubStateMachine.TryResetState(_attackState);
                    break;
                case SubStateType.Damage:
                    _damageSubStateMachine.TryResetState(_damageState);
                    break;
                case SubStateType.Death:
                    _damageSubStateMachine.TryResetState(_deathState);
                    break;
            }
        }
    }
}