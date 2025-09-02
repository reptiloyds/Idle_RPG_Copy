using PleasantlyGames.RPG.AndroidRuntime.Ad.Registration;
using PleasantlyGames.RPG.AndroidRuntime.Firebase;
using PleasantlyGames.RPG.AndroidRuntime.InApp.Registration;
using PleasantlyGames.RPG.AndroidRuntime.LoadUnits;
using PleasantlyGames.RPG.Runtime.Balance.Registration;
using PleasantlyGames.RPG.Runtime.Bootstrap;
using PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits;
using PleasantlyGames.RPG.Runtime.Localization.Registration;
using PleasantlyGames.RPG.Runtime.Pause.Registration;
using PleasantlyGames.RPG.Runtime.Save.Registration;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Registartion;
using PleasantlyGames.RPG.Runtime.User.Registration;
using PleasantlyGames.RPG.Runtime.Utilities.ApplicationCloser.Model;
using VContainer;

namespace PleasantlyGames.RPG.AndroidRuntime.Bootstrap
{
    public class AndroidBootstrapScope : BaseBootstrapScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            AdMobRegistration.Register(builder);
            AndroidInAppRegistration.Register(builder);
            builder.Register<FirebaseLoadUnit>(Lifetime.Scoped).AsSelf();
            
            MockSaveRegistration.Register(builder);
            MockUserRegistration.Register(builder);
            WebRequestTimeRegistration.Register(builder);
            
            MockPauseRegistration.Register(builder);
            MockLocalizationRegistration.Register(builder);
            MockBalanceRegistration.Register(builder);

            builder.Register<DefaultApplicationCloser>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<IBootstrapStarter, MockBootstrapStarter>(Lifetime.Scoped);
        }

        protected override CompositeLoadUnit CreatePlatformLoadUnit() => 
            new AndroidServicesLoadUnit("AndroidServicesLoading");
    }
}