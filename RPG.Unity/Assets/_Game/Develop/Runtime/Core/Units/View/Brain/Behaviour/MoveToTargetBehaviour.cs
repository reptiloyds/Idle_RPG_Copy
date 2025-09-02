using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Behaviour
{
    internal class MoveToTargetBehaviour : BehaviourMachine
    {
        public MoveToTargetBehaviour(UnitView unitView) : base(unitView)
        {
        }

        public override void Initialize()
        {
            var moveToSpawn = new MoveToTargetKey(UnitView);
            moveToSpawn.OnComplete(AchievePosition);
            StartNode = moveToSpawn;
        }

        private void AchievePosition() => 
            UnitView.AchieveMovementTarget();
    }
}