using PleasantlyGames.RPG.Runtime.User.Definition;
using PleasantlyGames.RPG.YGRuntime.User.Model;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.YGRuntime.User.Scope
{
    public class GamePushUserRegistration
    {
        public static void Register(IContainerBuilder builder, DeviceSimulationConfig configuration)
        {
            builder.Register<GamePushUserService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterComponent(configuration);
        }
    }
}