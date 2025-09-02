using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Behaviour
{
    internal class MoveForwardBehaviour : BehaviourMachine
    {
        public MoveForwardBehaviour(UnitView unitView) : base(unitView){}

        public override void Initialize()
        {
            StartNode = new MoveForward(UnitView);
        }
    }
}