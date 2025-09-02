using PleasantlyGames.RPG.Runtime.Balance.Registration;
using PleasantlyGames.RPG.YGRuntime.Balance.Model;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.Balance.Registration
{
    public class GamePushBalanceRegistration : BaseBalanceRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            builder.Register<GamePushBalanceProvider>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}