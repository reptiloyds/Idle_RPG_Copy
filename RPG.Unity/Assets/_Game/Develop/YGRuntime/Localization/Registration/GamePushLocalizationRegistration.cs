using PleasantlyGames.RPG.Runtime.Localization.Registration;
using PleasantlyGames.RPG.YGRuntime.Localization.Model;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.Localization.Registration
{
    public class GamePushLocalizationRegistration : BaseLocalizationRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            builder.Register<GamePushLanguageSource>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}