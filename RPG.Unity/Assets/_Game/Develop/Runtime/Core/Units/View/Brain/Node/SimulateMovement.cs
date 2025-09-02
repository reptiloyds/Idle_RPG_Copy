using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.States;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node
{
    internal class SimulateMovement : UnitNode
    {
        public SimulateMovement(UnitView unitView) : base(unitView)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            
            UnitView.BaseMovement.StartFakeMove();
            UnitView.StateMachine.SetSubState(SubStateType.Move);
        }

        public override void Exit()
        {
            base.Exit();
            
            UnitView.BaseMovement.StopFakeMove();
            UnitView.StateMachine.SetSubState(SubStateType.Idle);
        }
    }
}