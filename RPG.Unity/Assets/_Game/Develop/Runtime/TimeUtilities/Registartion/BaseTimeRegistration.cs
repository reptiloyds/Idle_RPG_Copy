using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Save;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.TimeUtilities.Registartion
{
    public class BaseTimeRegistration
    {
        protected static void BaseRegister(IContainerBuilder builder)
        {
            builder.Register<TimeDataProvider>(Lifetime.Singleton)
                .AsImplementedInterfaces().AsSelf();
            builder.Register<TimeService>(Lifetime.Singleton)
                .AsImplementedInterfaces().AsSelf();
        }
    }
}