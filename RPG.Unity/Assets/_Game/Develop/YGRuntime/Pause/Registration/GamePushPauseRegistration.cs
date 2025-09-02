using PleasantlyGames.RPG.Runtime.Pause.Registration;
using PleasantlyGames.RPG.YGRuntime.Pause.Model;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.Pause.Registration
{
    public class GamePushPauseRegistration : BasePauseRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<GamePushPause>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}