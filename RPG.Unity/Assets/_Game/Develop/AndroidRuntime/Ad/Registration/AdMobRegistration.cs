using PleasantlyGames.RPG.AndroidRuntime.Ad.Model;
using PleasantlyGames.RPG.AndroidRuntime.LoadUnits;
using PleasantlyGames.RPG.Runtime.Ad.Registration;
using VContainer;

namespace PleasantlyGames.RPG.AndroidRuntime.Ad.Registration
{
    public class AdMobRegistration : BaseAdRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            
            builder.Register<AdMobService>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            builder.Register<AdMobLoadUnit>(Lifetime.Scoped)
                .AsSelf();
            builder.Register<UmpService>(Lifetime.Singleton)
                .AsSelf();
        }
    }
}