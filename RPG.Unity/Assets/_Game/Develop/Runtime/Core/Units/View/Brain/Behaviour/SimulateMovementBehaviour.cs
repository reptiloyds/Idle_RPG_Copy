using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Behaviour
{
    internal class SimulateMovementBehaviour : BehaviourMachine
    {
        public SimulateMovementBehaviour(UnitView unitView) : base(unitView)
        {
        }

        public override void Initialize()
        {
            StartNode = new SimulateMovement(UnitView);
        }
    }
}