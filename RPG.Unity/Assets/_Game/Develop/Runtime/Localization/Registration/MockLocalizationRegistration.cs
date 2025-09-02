using PleasantlyGames.RPG.Runtime.Localization.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Localization.Registration
{
    public class MockLocalizationRegistration : BaseLocalizationRegistration
    {
        [Preserve]
        public MockLocalizationRegistration()
        {
            
        }
        
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            builder.Register<MockLanguageSource>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }
    }
}