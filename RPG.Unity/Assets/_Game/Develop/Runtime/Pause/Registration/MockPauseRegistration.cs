using PleasantlyGames.RPG.Runtime.Pause.Model;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Pause.Registration
{
    public class MockPauseRegistration : BasePauseRegistration
    {
        [Preserve]
        public MockPauseRegistration()
        {
        }
        
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<PauseService>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }
    }
}