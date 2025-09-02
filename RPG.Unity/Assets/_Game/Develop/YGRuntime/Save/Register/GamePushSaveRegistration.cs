using PleasantlyGames.RPG.Runtime.Save.Registration;
using PleasantlyGames.RPG.Runtime.Save.Serializers;
using PleasantlyGames.RPG.YGRuntime.Const;
using PleasantlyGames.RPG.YGRuntime.Save.Model;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.YGRuntime.Save.Register
{
    public class GamePushSaveRegistration : BaseSaveRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            BaseRegister(builder);
            builder.RegisterComponent(new JsonMigrationDataSerializer()).As<IDataSerializer>();
            builder.Register<GamePushDataRepository>(Lifetime.Singleton).WithParameter(GamePushVariables.Progress).AsImplementedInterfaces();
        }
    }
}