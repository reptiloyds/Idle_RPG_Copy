using PleasantlyGames.RPG.NutakuRuntime.Save.Model;
using PleasantlyGames.RPG.Runtime.Save.Registration;
using PleasantlyGames.RPG.Runtime.Save.Serializers;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.NutakuRuntime.Save.Register
{
    public class NutakuSaveRegistration : BaseSaveRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            builder.RegisterComponent(new JsonMigrationDataSerializer()).As<IDataSerializer>();
            builder.Register<NutakuDataRepository>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }
    }
}