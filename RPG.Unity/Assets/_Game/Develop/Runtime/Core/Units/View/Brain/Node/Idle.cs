using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.States;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node
{
    internal class Idle : UnitNode
    {
        private readonly float _idleDuration;
        private float _endTime;
        
        public Idle(UnitView unitView, float idleDuration) : base(unitView)
        {
            _idleDuration = idleDuration;
        }

        public override void Enter()
        {
            base.Enter();
            
            UnitView.StateMachine.SetSubState(SubStateType.Idle);
            _endTime = Time.time + _idleDuration;
        }

        public override void Update()
        {
            base.Update();
            
            if(Time.time < _endTime) return;
            Complete();
        }
    }
}