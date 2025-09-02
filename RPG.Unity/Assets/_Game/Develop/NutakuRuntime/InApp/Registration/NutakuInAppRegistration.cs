using PleasantlyGames.RPG.NutakuRuntime.InApp.Model;
using VContainer;

namespace PleasantlyGames.RPG.NutakuRuntime.InApp.Registration
{
    public class NutakuInAppRegistration
    {
        public static void Register(IContainerBuilder builder) => 
            builder.Register<NutakuInAppProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
    }
}