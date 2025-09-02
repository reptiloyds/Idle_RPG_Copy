using PleasantlyGames.RPG.YGRuntime.InAppPurchase.Model;
using PleasantlyGames.RPG.YGRuntime.LoadUnits;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.InAppPurchase.Registration
{
    public class GamePushIAPRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<GamePushInAppProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GamePushInAppLoadUnit>(Lifetime.Singleton)
                .AsSelf();
        }
    }
}