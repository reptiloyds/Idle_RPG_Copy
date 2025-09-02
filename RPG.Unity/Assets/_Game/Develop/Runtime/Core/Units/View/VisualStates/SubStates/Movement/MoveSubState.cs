namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.SubStates.Movement
{
    public class MoveSubState : VisualState<SubStateType>
    {
        public override SubStateType Type => SubStateType.Move;
        protected override bool CanInterruptSelf => true;
    }
}