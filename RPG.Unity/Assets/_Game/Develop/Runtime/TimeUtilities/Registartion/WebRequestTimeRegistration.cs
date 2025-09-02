using PleasantlyGames.RPG.Runtime.TimeUtilities.LoadUnits;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.TimeUtilities.Registartion
{
    public class WebRequestTimeRegistration : BaseTimeRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            
            builder.Register<WebRequestTimeLoadUnit>(Lifetime.Singleton).AsSelf();
        }
    }
}