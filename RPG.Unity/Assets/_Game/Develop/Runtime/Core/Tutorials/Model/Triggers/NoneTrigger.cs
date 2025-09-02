namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers
{
    public class NoneTrigger : TutorialTrigger
    {
        public override void Initialize() => 
            Execute();

        public override void Dispose()
        {
        }
    }
}