//#define RPG_PROD
using _Game.Scripts.Systems.Nutaku;
using _Game.Scripts.Systems.Server;
using NutakuUnitySdk;
using PleasantlyGames.RPG.NutakuRuntime.Analytics.Model;
using PleasantlyGames.RPG.NutakuRuntime.InApp.Registration;
using PleasantlyGames.RPG.NutakuRuntime.LoadUnits;
using PleasantlyGames.RPG.NutakuRuntime.Save.Register;
using PleasantlyGames.RPG.NutakuRuntime.TimeRegistration;
using PleasantlyGames.RPG.Runtime.Ad.Registration;
using PleasantlyGames.RPG.Runtime.Analytics.Save;
using PleasantlyGames.RPG.Runtime.Balance.Registration;
using PleasantlyGames.RPG.Runtime.Bootstrap;
using PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits;
using PleasantlyGames.RPG.Runtime.Device;
using PleasantlyGames.RPG.Runtime.Localization.Registration;
using PleasantlyGames.RPG.Runtime.Pause.Registration;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.NutakuRuntime.Bootstrap
{
    public class NutakuBootstrapScope : BaseBootstrapScope
    {
        [SerializeField] private string _prodServerUrl = "https://prod.amazed.games";
        [SerializeField] private string _devServerUrl = "https://dev.amazed.games";
        [SerializeField] private string _tittleId = "idle-rpg";
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

#if UNITY_ANDROID
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
#endif
            
            NutakuSaveRegistration.Register(builder);
            NutakuInAppRegistration.Register(builder);
            MockAdRegistration.Register(builder);
            NutakuTimeRegistration.Register(builder);
            
            MockPauseRegistration.Register(builder);
            MockLocalizationRegistration.Register(builder);
            BalanceRegistration.Register(builder);

            
#if UNITY_ANDROID
#if RPG_DEV
            NutakuSdkConfig.Environment = "SANDBOX";
#elif RPG_PROD
            NutakuSdkConfig.Environment = "PRODUCTION";
#endif
            NutakuSdkConfig.TitleId = _tittleId;
            NutakuSdkConfig.AndroidPackageName = Application.identifier;
#endif
            
            builder.RegisterComponentOnNewGameObject<NutakuSystem>(Lifetime.Singleton, "NutakuSystem")
                .DontDestroyOnLoad()
                .AsImplementedInterfaces()
                .AsSelf();
            
            builder.RegisterComponent(new MockDeviceService(_deviceType))
                .AsImplementedInterfaces();
            builder.Register<NutakuLoadUnit>(Lifetime.Scoped).AsSelf();
            builder.Register<ServerSystem>(Lifetime.Singleton)
                .AsImplementedInterfaces().AsSelf();
#if RPG_PROD
            builder.Register<ServerApi>(Lifetime.Singleton)
                .WithParameter(_prodServerUrl)
                .AsImplementedInterfaces()
                .AsSelf();
#elif RPG_DEV
            builder.Register<ServerApi>(Lifetime.Singleton)
                .WithParameter(_devServerUrl)
                .AsImplementedInterfaces()
                .AsSelf();
#endif
            
            builder.Register<ServerLoadUnit>(Lifetime.Scoped).AsSelf();
            builder.Register<CheckVersionLoadUnit>(Lifetime.Scoped).AsSelf();
            builder.Register<IBootstrapStarter, MockBootstrapStarter>(Lifetime.Scoped);
            
            builder.Register<AnalyticsDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<NutakuAnalyticsService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        protected override CompositeLoadUnit CreatePlatformLoadUnit() => 
            new NutakuServicesLoadUnit("NutakuServicesLoading");
    }
}