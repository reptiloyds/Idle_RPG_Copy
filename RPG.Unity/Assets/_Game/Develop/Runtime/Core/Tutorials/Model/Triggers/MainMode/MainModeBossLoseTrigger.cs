namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.MainMode
{
    internal class MainModeBossLoseTrigger : TutorialTrigger
    {
        private readonly GameMode.Model.Main.MainMode _mainMode;

        public MainModeBossLoseTrigger(GameMode.Model.Main.MainMode mainMode) => 
            _mainMode = mainMode;

        public override void Initialize() => 
            _mainMode.OnBoseLose += OnBoseLose;

        public override void Dispose() => 
            _mainMode.OnBoseLose -= OnBoseLose;

        private void OnBoseLose() => 
            Execute();
    }
}