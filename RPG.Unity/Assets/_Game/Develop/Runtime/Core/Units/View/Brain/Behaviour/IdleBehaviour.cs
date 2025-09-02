namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Behaviour
{
    public class IdleBehaviour : BehaviourMachine
    {
        public IdleBehaviour(UnitView unitView) : base(unitView)
        {
        }

        public override void Initialize() => 
            StartNode = new Node.Idle(UnitView, 10);
    }
}