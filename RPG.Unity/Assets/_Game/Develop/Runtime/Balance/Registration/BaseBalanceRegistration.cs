using PleasantlyGames.RPG.Runtime.Balance.Save;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Balance.Registration
{
    public class BaseBalanceRegistration
    {
        protected static void BaseRegister(IContainerBuilder builder)
        {
            builder.Register<BalanceDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}