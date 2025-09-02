using PleasantlyGames.RPG.YGRuntime.Segments.Model;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.Segments.Registration
{
    public class GamePushSegmentRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<GamePushSegmentService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}