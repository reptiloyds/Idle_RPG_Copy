namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.SubStates.Damage
{
    public class DamageSubState : VisualState<SubStateType>
    {
        public override SubStateType Type => SubStateType.Damage;
        public override StatePriority Priority => StatePriority.Medium;
        
        protected override bool CanInterruptSelf => true;
    }
}