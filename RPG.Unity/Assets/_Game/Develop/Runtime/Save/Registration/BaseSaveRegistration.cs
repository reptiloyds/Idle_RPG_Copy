using PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Save.Registration
{
    public class BaseSaveRegistration
    {
        protected static void BaseRegister(IContainerBuilder builder)
        {
            builder.Register<ProgressDataLoadUnit>(Lifetime.Scoped)
                .AsImplementedInterfaces().AsSelf();
        }
    }
}