using PleasantlyGames.RPG.Runtime.Save.Models;
using PleasantlyGames.RPG.Runtime.Save.Serializers;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Save.Registration
{
    public class MockSaveRegistration : BaseSaveRegistration
    {
        [Preserve]
        public MockSaveRegistration()
        {
        }
        
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            builder.RegisterComponent(new JsonMigrationDataSerializer()).As<IDataSerializer>();
            builder.Register<PlayerPrefsDataRepository>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}