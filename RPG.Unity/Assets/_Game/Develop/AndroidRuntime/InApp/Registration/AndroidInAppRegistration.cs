using PleasantlyGames.RPG.AndroidRuntime.InApp.Model;
using PleasantlyGames.RPG.AndroidRuntime.LoadUnits;
using VContainer;

namespace PleasantlyGames.RPG.AndroidRuntime.InApp.Registration
{
    public class AndroidInAppRegistration
    {
        [Preserve]
        public AndroidInAppRegistration()
        {
        }

        public static void Register(IContainerBuilder builder)
        {
            builder.Register<UnityPurchaseProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UnityPurchaseDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UnityPurchaseLoadUnit>(Lifetime.Scoped).AsSelf();
        }
    }
}