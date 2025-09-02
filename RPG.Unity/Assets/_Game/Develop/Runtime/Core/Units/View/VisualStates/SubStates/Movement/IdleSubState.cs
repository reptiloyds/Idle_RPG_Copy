namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.SubStates.Movement
{
    public class IdleSubState : VisualState<SubStateType>
    {
        public override SubStateType Type => SubStateType.Idle;
        protected override bool CanInterruptSelf => true;
    }
}