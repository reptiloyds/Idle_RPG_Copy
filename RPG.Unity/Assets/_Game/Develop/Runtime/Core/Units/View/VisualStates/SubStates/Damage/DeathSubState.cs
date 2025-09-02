namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.SubStates.Damage
{
    public class DeathSubState : VisualState<SubStateType>
    {
        public override SubStateType Type => SubStateType.Death;
        public override StatePriority Priority => StatePriority.High;
        public override bool CanExitState => true;
        protected override bool CanInterruptSelf => true;
    }
}