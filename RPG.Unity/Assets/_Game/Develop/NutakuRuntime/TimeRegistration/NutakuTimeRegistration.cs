using PleasantlyGames.RPG.NutakuRuntime.LoadUnits;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Registartion;
using VContainer;

namespace PleasantlyGames.RPG.NutakuRuntime.TimeRegistration
{
    public class NutakuTimeRegistration : BaseTimeRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            
            builder.Register<NutakuTimeLoadUnit>(Lifetime.Singleton).AsSelf();
        }
    }
}