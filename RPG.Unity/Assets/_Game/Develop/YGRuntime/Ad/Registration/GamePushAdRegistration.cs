using PleasantlyGames.RPG.Runtime.Ad.Registration;
using PleasantlyGames.RPG.YGRuntime.Ad.Model;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.Ad.Registration
{
    public class GamePushAdRegistration : BaseAdRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            
            builder.Register<GamePushAdService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}