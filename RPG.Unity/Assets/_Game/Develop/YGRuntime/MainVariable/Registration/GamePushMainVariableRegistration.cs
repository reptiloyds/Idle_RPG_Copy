using PleasantlyGames.RPG.YGRuntime.MainVariable.Model;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.MainVariable.Registration
{
    public class GamePushMainVariableRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<GamePushMainVariableHandler>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}