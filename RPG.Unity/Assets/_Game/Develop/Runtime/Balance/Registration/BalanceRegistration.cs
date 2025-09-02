using PleasantlyGames.RPG.Runtime.Balance.Model;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Balance.Registration
{
    public class BalanceRegistration : BaseBalanceRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            builder.Register<BalanceProvider>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }
    }
}