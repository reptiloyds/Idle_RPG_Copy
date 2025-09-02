using GamePush;
using PleasantlyGames.RPG.Runtime.Bootstrap;
using PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits;
using PleasantlyGames.RPG.Runtime.User.Definition;
using PleasantlyGames.RPG.Runtime.Utilities.ApplicationCloser.Model;
using PleasantlyGames.RPG.YGRuntime.Ad.Registration;
using PleasantlyGames.RPG.YGRuntime.Balance.Registration;
using PleasantlyGames.RPG.YGRuntime.InAppPurchase.Registration;
using PleasantlyGames.RPG.YGRuntime.LoadUnits;
using PleasantlyGames.RPG.YGRuntime.Localization.Registration;
using PleasantlyGames.RPG.YGRuntime.MainVariable.Registration;
using PleasantlyGames.RPG.YGRuntime.Pause.Registration;
using PleasantlyGames.RPG.YGRuntime.Save.Register;
using PleasantlyGames.RPG.YGRuntime.Segments.Registration;
using PleasantlyGames.RPG.YGRuntime.TimeUtility.Register;
using PleasantlyGames.RPG.YGRuntime.User.Scope;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.YGRuntime.Bootstrap
{
    public class YGBootstrapScope : BaseBootstrapScope
    {
        [SerializeField] private BootstrapWindow _bootstrapWindow;
        
        protected override void Awake() => 
            WaitForWebGLBoot();

        private void WaitForWebGLBoot()
        {
            if (GP_Init.isReady)
                Build();
            else
            {
                GP_Init.OnReady += OnGamePushReady;
                GP_Init.OnError += OnGamePushError;
            }
        }

        private void OnGamePushReady()
        {
            GP_Init.OnReady -= OnGamePushReady;
            GP_Init.OnError -= OnGamePushError;
            Build();
        }

        private void OnGamePushError()
        {
            GP_Init.OnReady -= OnGamePushReady;
            GP_Init.OnError -= OnGamePushError;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
#if UNITY_EDITOR
            GP_Settings.instance.platformSettings = Resources.Load<GP_PlatformSettings>("GP_PlatformSettings");      
#endif
            GamePushAdRegistration.Register(builder);
            GamePushLocalizationRegistration.Register(builder);
            GamePushPauseRegistration.Register(builder);
            GamePushSaveRegistration.Register(builder);
            GamePushTimeRegistration.Register(builder);
            GamePushUserRegistration.Register(builder, new DeviceSimulationConfig(false));
            GamePushIAPRegistration.Register(builder);
            GamePushBalanceRegistration.Register(builder);
            GamePushSegmentRegistration.Register(builder);
            GamePushMainVariableRegistration.Register(builder);

            builder.Register<DefaultApplicationCloser>(Lifetime.Singleton).AsImplementedInterfaces();
            
            if (_bootstrapWindow != null)
                builder.RegisterComponent(_bootstrapWindow).AsImplementedInterfaces();
            else
                builder.Register<IBootstrapStarter, MockBootstrapStarter>(Lifetime.Scoped);
        }

        protected override CompositeLoadUnit CreatePlatformLoadUnit() =>
            new WebServicesLoadUnit("WebServicesLoading");
    }
}