using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.View;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.UI;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.View;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.View;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.View;
using PleasantlyGames.RPG.Runtime.Core.Quests.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.View;
using PleasantlyGames.RPG.Runtime.Core.Skill.View.UI;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Accent.View;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Monologue.View;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Backpack;
using PleasantlyGames.RPG.Runtime.Core.UI.Hub;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.UI;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Save;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.UI;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.UI.StatImprove;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Health;
using PleasantlyGames.RPG.Runtime.DI.Attributes;
using PleasantlyGames.RPG.Runtime.DI.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.UI
{
    public class CoreUIScope : AutoLifetimeScope
    {
        [SerializeField, AutoFill, Required] private MainModeView _mainModeView;
        [SerializeField, AutoFill, Required] private StatImproverPage _statImproverPage;
        [SerializeField, AutoFill, Required] private MessageBufferView _messageBufferView;
        [SerializeField, AutoFill, Required] private PowerDeltaPresenter _powerDeltaPresenter;
        [SerializeField, AutoFill, Required] private UIHub _hub;
        [SerializeField, AutoFill, Required] private SkillItemCasterView _skillCasterView;
        [SerializeField, AutoFill, Required] private QuestView _questView;
        [SerializeField, AutoFill, Required] private MonologuePresenter _monologuePresenter;
        [SerializeField, AutoFill, Required] private PointerPresenter _pointerPresenter;
        [SerializeField, AutoFill, Required] private AccentPresenter _accentPresenter;
        [SerializeField, AutoFill, Required] private BossRushView _bossRushView;
        [SerializeField, AutoFill, Required] private BossRushHubPage _bossRushHubPage;
        [SerializeField, AutoFill, Required] private SoftRushView _softRushView;
        [SerializeField, AutoFill, Required] private SoftRushHubPage _softRushHubPage;
        [SerializeField, AutoFill, Required] private ResourceBackpackView _resourceBackpackView;
        [SerializeField, AutoFill, Required] private SubModesPage _subModesPage;
        [SerializeField, AutoFill, Required] private BlessingButton _blessingButton;

        private readonly UniTaskCompletionSource _initializeCompletionSource = new();
        
        public async UniTask InitializeAsync() => 
            await _initializeCompletionSource.Task;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponent(_mainModeView).AsImplementedInterfaces();
            builder.RegisterComponent(_statImproverPage).AsImplementedInterfaces();
            builder.RegisterComponent(_messageBufferView).AsImplementedInterfaces();
            builder.RegisterComponent(_powerDeltaPresenter).AsImplementedInterfaces();
            builder.RegisterComponent(_hub).AsImplementedInterfaces();
            builder.RegisterComponent(_skillCasterView).AsImplementedInterfaces();
            builder.RegisterComponent(_questView).AsImplementedInterfaces();
            builder.RegisterComponent(_monologuePresenter).AsImplementedInterfaces();
            builder.RegisterComponent(_pointerPresenter).AsImplementedInterfaces();
            builder.RegisterComponent(_accentPresenter).AsImplementedInterfaces();
            builder.RegisterComponent(_bossRushView).AsImplementedInterfaces();
            builder.RegisterComponent(_bossRushHubPage).AsImplementedInterfaces();
            builder.RegisterComponent(_softRushView).AsImplementedInterfaces();
            builder.RegisterComponent(_softRushHubPage).AsImplementedInterfaces();
            builder.RegisterComponent(_resourceBackpackView).AsSelf();
            builder.RegisterComponent(_subModesPage).AsImplementedInterfaces();
            builder.RegisterComponent(_blessingButton).AsImplementedInterfaces();
            
            builder.Register<ResourceFXFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<NotificationService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<NotificationDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.RegisterEntryPoint<CoreUIFlow>().WithParameter(_initializeCompletionSource);
        }

        protected override void AutoFill()
        {
            base.AutoFill();
            
            AddAutoInjectedObject<UICoreRoot>();
        }
    }
}