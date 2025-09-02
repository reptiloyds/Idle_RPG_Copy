namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.SubStates.Combat
{
    public class AttackSubState : VisualState<SubStateType>
    {
        public override SubStateType Type => SubStateType.Attack;
        public override StatePriority Priority => StatePriority.Medium;
        public override bool CanExitState => true;

        protected override bool CanInterruptSelf => true;
    }
}