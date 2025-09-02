using PleasantlyGames.RPG.Runtime.Localization.LoadUnits;
using PleasantlyGames.RPG.Runtime.Localization.Models.I2;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Localization.Registration
{
    public class BaseLocalizationRegistration
    {
        protected static void BaseRegister(IContainerBuilder builder)
        {
            builder.Register<I2Translator>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<TranslationLoadUnit>(Lifetime.Singleton).AsSelf();
        }
    }
}