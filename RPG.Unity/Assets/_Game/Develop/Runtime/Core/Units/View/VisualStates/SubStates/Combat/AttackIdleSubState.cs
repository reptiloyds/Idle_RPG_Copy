namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.SubStates.Combat
{
    public class AttackIdleSubState : VisualState<SubStateType>
    {
        public override SubStateType Type => SubStateType.AttackIdle;
        public override StatePriority Priority => StatePriority.Medium;
        public override bool CanExitState => true;
        
        protected override bool CanInterruptSelf => true;
    }
}