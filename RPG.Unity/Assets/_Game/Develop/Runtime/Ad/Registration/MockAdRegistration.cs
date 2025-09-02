using PleasantlyGames.RPG.Runtime.Ad.Model.Mock;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Ad.Registration
{
    public class MockAdRegistration : BaseAdRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            
            builder.Register<MockAdService>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }
    }
}