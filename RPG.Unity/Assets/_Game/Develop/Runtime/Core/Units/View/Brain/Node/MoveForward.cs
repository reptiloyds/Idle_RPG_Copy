using PleasantlyGames.RPG.Runtime.Core.Units.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node
{
    internal class MoveForward : UnitNode
    {
        private const float MoveDistance = 15f;

        public MoveForward(UnitView unitView) : base(unitView){}

        public override void Enter()
        {
            base.Enter();
            UnitView.BaseMovement.OnStop += Move;
            Move();
        }
        
        public override void Exit()
        {
            base.Exit();
            
            UnitView.BaseMovement.ResetFakeMove();
            UnitView.BaseMovement.OnStop -= Move;
        }

        private void Move()
        {
            var result = UnitView.GetLookDirection(UnitPointKey.Spawn);
            UnitView.BaseMovement.StartFakeMove();
            UnitView.BaseMovement.MoveTo(UnitView.transform.position + result.direction * MoveDistance);
            UnitView.Rotator.Look(result.direction);
        }
    }
}