using PleasantlyGames.RPG.Runtime.InAppPurchase.Model;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.InAppPurchase.Registration
{
    public class MockInAppRegistration
    {
        [Preserve]
        public MockInAppRegistration()
        {
            
        }
        
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<MockInAppProvider>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }
    }
}