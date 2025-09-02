using GamePush;
using PleasantlyGames.RPG.Runtime.Pause.Definition;
using PleasantlyGames.RPG.Runtime.Pause.Model;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.Pause.Model
{
    public class GamePushPause : PauseService
    {
        [Inject] private PauseConfiguration _pauseConfiguration;

        [Preserve]
        public GamePushPause()
        { }
        
        public override void Initialize()
        {
            base.Initialize();

            GP_Game.OnPause += OnPaused;
            GP_Game.OnResume += OnResumed;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            GP_Game.OnPause -= OnPaused;
            GP_Game.OnResume -= OnResumed;
        }

        private void OnPaused() => 
            Pause(_pauseConfiguration.Ad);

        private void OnResumed() => 
            Continue(_pauseConfiguration.Ad);
    }
}
