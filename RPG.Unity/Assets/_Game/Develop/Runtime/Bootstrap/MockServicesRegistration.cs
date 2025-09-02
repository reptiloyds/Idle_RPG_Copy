using PleasantlyGames.RPG.Runtime.Ad.Registration;
using PleasantlyGames.RPG.Runtime.Balance.Registration;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Registration;
using PleasantlyGames.RPG.Runtime.Localization.Registration;
using PleasantlyGames.RPG.Runtime.Pause.Registration;
using PleasantlyGames.RPG.Runtime.Save.Registration;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Registartion;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Bootstrap
{
    public static class MockServicesRegistration
    {
        public static void Configure(IContainerBuilder builder)
        {
            MockSaveRegistration.Register(builder);
            MockAdRegistration.Register(builder);
            WebRequestTimeRegistration.Register(builder);
            MockPauseRegistration.Register(builder);
            MockLocalizationRegistration.Register(builder);
            MockInAppRegistration.Register(builder);
            BalanceRegistration.Register(builder);
            
            builder.Register<IBootstrapStarter, MockBootstrapStarter>(Lifetime.Scoped);
        }
    }
}