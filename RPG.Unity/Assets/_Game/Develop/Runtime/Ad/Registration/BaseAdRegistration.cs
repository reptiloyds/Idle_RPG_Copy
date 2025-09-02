using PleasantlyGames.RPG.Runtime.Ad.Save;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Ad.Registration
{
    public class BaseAdRegistration
    {
        protected static void BaseRegister(IContainerBuilder builder)
        {
            builder.RegisterComponent(new AdDataProvider()).AsImplementedInterfaces().AsSelf();
        }
    }
}