using PleasantlyGames.RPG.Runtime.TimeUtilities.Registartion;
using PleasantlyGames.RPG.YGRuntime.TimeUtility.LoadUnit;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.TimeUtility.Register
{
    public class GamePushTimeRegistration : BaseTimeRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            builder.Register<GamePushServerTimeLoadUnit>(Lifetime.Singleton).AsSelf();
        }
    }
}