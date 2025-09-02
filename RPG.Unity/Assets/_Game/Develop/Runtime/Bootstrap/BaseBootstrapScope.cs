using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.AssetManagement.Download;
using PleasantlyGames.RPG.Runtime.AssetManagement.LoadUnits;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using PleasantlyGames.RPG.Runtime.Balance.Contract;
using PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits;
using PleasantlyGames.RPG.Runtime.Core.Balance;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Configs;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model;
using PleasantlyGames.RPG.Runtime.DI.Base;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.LoadUnit;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.Model;
using PleasantlyGames.RPG.Runtime.Save.Models;
using PleasantlyGames.RPG.Runtime.Save.Snapshot;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PleasantlyGames.RPG.Runtime.Utilities.Coroutine;
using PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.Model;
using PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.View;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.Model;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.View;
using PleasantlyGames.RPG.Runtime.VIP.Contract;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using PleasantlyGames.RPG.Runtime.VIP.Save;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DeviceType = PleasantlyGames.RPG.Runtime.Device.DeviceType;

namespace PleasantlyGames.RPG.Runtime.Bootstrap
{
    [HideMonoScript]
    public abstract class BaseBootstrapScope : AutoLifetimeScope
    {
        [SerializeField, Required] private GameConfig GameConfig;
        [SerializeField, Required] private SnapshotConfig SnapshotConfig;
        [SerializeField, Required] private CoroutineRunner _coroutineRunner;
        [SerializeField] protected DeviceType _deviceType;

        private void Reset() => FindComponents();

        private void OnValidate() => FindComponents();

        private void FindComponents() => 
            _coroutineRunner ??= FindAnyObjectByType<CoroutineRunner>();

        protected override void Awake()
        {
            DontDestroyOnLoad(this);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            StringExtension.Build(GameConfig.BigValueConfiguration.Postifixes);
            builder.RegisterComponent(SnapshotConfig);

            builder.RegisterComponent(_coroutineRunner);
            BindGameConfig(builder);

            builder.Register<PrivacyPolicyService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PrivacyPolicyLoadUnit>(Lifetime.Singleton).AsSelf();
            builder.Register<TechnicalMessageService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<InternetConnectionService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<InternetConnectionMediator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SaveService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ThirdPartyEvents>(Lifetime.Singleton).AsSelf();
            builder.Register<ThirdPartyWrongMediator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<LoadingService>(Lifetime.Scoped);
            builder.Register<LoadingScreenProvider>(Lifetime.Singleton);

            builder.Register<IAudioService, AudioService>(Lifetime.Singleton);
            builder.Register<AudioPauseHandler>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<ButtonService>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<IAssetDownloader, AssetDownloader>(Lifetime.Singleton);
            builder.Register<UpdateCatalogLoadUnit>(Lifetime.Singleton).AsSelf();
            builder.Register<IAssetDownloadReporter, AssetDownloadReporter>(Lifetime.Singleton);
            builder.Register<IAssetProvider, AssetProvider>(Lifetime.Singleton);
            builder.Register<AssetTechnicalHandler>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ISpriteProvider, SpriteProvider>(Lifetime.Singleton);

            builder.Register<BalanceContainer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<IBalanceLoader, BalanceLoader>(Lifetime.Singleton);

            builder.Register<UIFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<BalanceLoadUnit>(Lifetime.Scoped).AsSelf();
            builder.RegisterComponent(CreatePlatformLoadUnit()).AsImplementedInterfaces().AsSelf();
            builder.RegisterEntryPoint<BootstrapFlow>();
        }

        protected abstract CompositeLoadUnit CreatePlatformLoadUnit();

        private void BindGameConfig(IContainerBuilder builder)
        {
            builder.RegisterComponent(GameConfig.PauseConfiguration);
            builder.RegisterComponent(GameConfig.SaveConfiguration);
            builder.RegisterComponent(GameConfig.BalanceConfiguration);
            builder.RegisterComponent(GameConfig.AssetProviderDefinition);
            builder.RegisterComponent(GameConfig.AdDefinition);
            builder.RegisterComponent(GameConfig.MainModeConfiguration);
            builder.RegisterComponent(GameConfig.DungeonConfiguration);
            builder.RegisterComponent(GameConfig.BossRushConfiguration);
            builder.RegisterComponent(GameConfig.SoftRushConfiguration);
            builder.RegisterComponent(GameConfig.SlotRushConfiguration);
            builder.RegisterComponent(GameConfig.ItemConfiguration);
            builder.RegisterComponent(GameConfig.LootboxConfiguration);
            builder.RegisterComponent(GameConfig.PassiveIncomeConfiguration);
            builder.RegisterComponent(GameConfig.NotificationConfiguration);
            builder.RegisterComponent(GameConfig.BigValueConfiguration);
            builder.RegisterComponent(GameConfig.SkillConfiguration);
            builder.RegisterComponent(GameConfig.ResourceConfiguration);
            builder.RegisterComponent(GameConfig.AudioConfiguration);
            builder.RegisterComponent(GameConfig.MusicConfiguration);
            builder.RegisterComponent(GameConfig.CheatConfiguration);
            builder.RegisterComponent(GameConfig.InternetConnectionConfiguration);
            builder.RegisterComponent(GameConfig.PrivacyPolicyConfiguration);
            builder.RegisterComponent(GameConfig.ChatDefinition);
        }
    }
}