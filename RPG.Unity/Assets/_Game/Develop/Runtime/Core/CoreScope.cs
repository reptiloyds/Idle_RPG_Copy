using PleasantlyGames.RPG.Runtime.AssetManagement.Forecast;
using PleasantlyGames.RPG.Runtime.Core.Ally;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Save;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.Save;
using PleasantlyGames.RPG.Runtime.Core.Camera;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Save;
using PleasantlyGames.RPG.Runtime.Core.Characters.View.Mediator;
using PleasantlyGames.RPG.Runtime.Core.Cheats;
using PleasantlyGames.RPG.Runtime.Core.Collections.Model;
using PleasantlyGames.RPG.Runtime.Core.Collections.Save;
using PleasantlyGames.RPG.Runtime.Core.Companion;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Save;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Save;
using PleasantlyGames.RPG.Runtime.Core.Effects.Model;
using PleasantlyGames.RPG.Runtime.Core.Enemy;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.UI.Mediator;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.View;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.View;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Mediator;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.UI.Mediator;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Save;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Save;
using PleasantlyGames.RPG.Runtime.Core.Music.Model;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Model;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Save;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.View.Mediator;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Save;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.View;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Save;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Save;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Factories;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Save;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Services;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Save;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Service;
using PleasantlyGames.RPG.Runtime.Core.PopupNumbers.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Offers.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Offers.View.Mediator;
using PleasantlyGames.RPG.Runtime.Core.Products.Save;
using PleasantlyGames.RPG.Runtime.Core.Projectile.Model;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;
using PleasantlyGames.RPG.Runtime.Core.Quests.Save;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Save;
using PleasantlyGames.RPG.Runtime.Core.Resource.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Save;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.View.UI.Mediator;
using PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Save;
using PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Service;
using PleasantlyGames.RPG.Runtime.Core.Story;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Accent.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.ImproveHint.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Monologue.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.TutorialAnimation;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Save;
using PleasantlyGames.RPG.Runtime.Core.UI.Hub.Sides;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Model;
using PleasantlyGames.RPG.Runtime.Core.UnitRendering;
using PleasantlyGames.RPG.Runtime.Core.Units.Factory;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Save;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Save;
using PleasantlyGames.RPG.Runtime.Core.Units.Team;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Health;
using PleasantlyGames.RPG.Runtime.DI.Attributes;
using PleasantlyGames.RPG.Runtime.DI.Base;
using PleasantlyGames.RPG.Runtime.Settings.Model;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.VIP.Contract;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using PleasantlyGames.RPG.Runtime.VIP.Save;
using PleasantlyGames.RPG.Runtime.VIP.View;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core
{
    public class CoreScope : AutoLifetimeScope
    {
        [SerializeField, AutoFill, Required] private CameraShaker _cameraShaker;
        [SerializeField, AutoFill, Required] private CameraPositioner _cameraPositioner;
        [SerializeField, AutoFill, Required] private CameraFOVAdapter _cameraFOVAdapter;
        [SerializeField, AutoFill, Required] private UnitRenderer _unitRenderer;

        private void ConfigureInfrastructure(IContainerBuilder builder)
        {
            builder.Register<CheatService>(Lifetime.Singleton)
                .AsImplementedInterfaces().AsSelf();
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            ConfigureInfrastructure(builder);
            
            builder.Register<ResourceService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ResourceDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.RegisterComponent(_cameraFOVAdapter).AsImplementedInterfaces();
            builder.Register<MortalUnitContainer>(Lifetime.Singleton).AsSelf();
            
            builder.Register<UnitStatService>(Lifetime.Singleton).AsSelf();
            builder.Register<UnitStatDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<BranchService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<BranchDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<CompanionSquad>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<AllySquad>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<EnemySquad>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<UnitFactory>(Lifetime.Singleton).AsSelf();
            builder.Register<LocationUnitCollection>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<LocationFactory>(Lifetime.Singleton).AsSelf();
            
            builder.Register<MainMode>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<MainModeDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UIBossPresentMediator>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<BossRushMode>(Lifetime.Singleton).As<DungeonMode>().AsImplementedInterfaces().AsSelf();
            builder.Register<BossRushDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UIDungeonWinMediator<BossRushMode, BossRushWinWindow>>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<SoftRushMode>(Lifetime.Singleton).As<DungeonMode>().AsImplementedInterfaces().AsSelf();
            builder.Register<SoftRushDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UIDungeonWinMediator<SoftRushMode, SoftRushWinWindow>>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<DungeonModeFacade>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<GameModeSwitcher>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UIGameModeResultMediator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<StatImprover>(Lifetime.Singleton).AsSelf();
            builder.Register<StatImproverDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PowerCalculator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<MessageBuffer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<PopupTextFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<ProjectileFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<EffectFactory>(Lifetime.Singleton).AsSelf();

            builder.Register<ItemDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ItemFacade>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<StuffDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<StuffInventory>(Lifetime.Singleton).As<BaseInventory<StuffItem>>()
                .AsImplementedInterfaces().AsSelf();
            
            builder.Register<CompanionDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<CompanionInventory>(Lifetime.Singleton).As<BaseInventory<CompanionItem>>()
                .AsImplementedInterfaces().AsSelf();
            
            builder.Register<SkillDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SkillInventory>(Lifetime.Singleton).As<BaseInventory<SkillItem>>()
                .AsImplementedInterfaces().AsSelf();
            
            builder.Register<ItemSkillService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<CharacterSkillService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<SkillFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SkillViewFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UISkillItemCasterMediator>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SkillTargetProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.RegisterComponent(_cameraShaker).AsImplementedInterfaces().AsSelf();
            builder.RegisterComponent(_cameraPositioner).AsSelf();
            
            builder.Register<GlobalStatProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<CharacterService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<CharacterDataProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<UICharacterUpMediator>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterComponent(_unitRenderer).AsImplementedInterfaces().AsSelf();
            
            builder.Register<QuestService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<QuestDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<TutorialService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<TutorialDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<TutorialAnimationLauncher>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<MonologueService>(Lifetime.Singleton).AsSelf();
            builder.Register<PointerService>(Lifetime.Singleton).AsSelf();
            builder.Register<AccentService>(Lifetime.Singleton).AsSelf();
            
            builder.Register<TimePauseHandler>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<ImproveHintMediator>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<LootboxDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<LootboxService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<ProductDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ProductService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PurchaseWindowPresenter>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ProductOfferService>(Lifetime.Singleton);
            builder.Register<UIProductOfferMediator>(Lifetime.Singleton);

            builder.Register<PassiveIncomeDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PassiveIncomeModel>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UIPassiveIncomeMediator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<CollectionDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<CollectionService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<DailyRoulette>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<DailyRouletteDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<RouletteFacade>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SlotRushDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SlotRushRoulette>(Lifetime.Singleton).As<RouletteMode>().AsImplementedInterfaces().AsSelf();

            builder.Register<PopupResourceFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<PeriodicRewardService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PeriodicRewardDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UIDailyLoginRewardMediator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<BlessingService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<BlessingDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<StoryService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<StoryDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<MusicLauncher>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<SettingsService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<SidesContainer>(Lifetime.Singleton);

            builder.Register<DailyService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<DailyDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<WindowService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ContentService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ContentDataProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<AssetForecaster>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<ResourceViewService>(Lifetime.Singleton).AsSelf();
            builder.Register<HealthBarFactory>(Lifetime.Singleton).AsSelf();
            
            builder.Register<ChatDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ChatService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<PiggyBankDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PiggyBankService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PiggyBankBonusesDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PiggyBankBonusService>(Lifetime.Singleton).AsSelf();
            builder.Register<PiggyBankBonusViewFactory>(Lifetime.Singleton).AsSelf();

            builder.Register<GalleryService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<GalleryDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<StageRewardsDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<StageRewardsService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<VipDataProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<VipService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<VipExtensionMediator>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<TooltipFactory>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.RegisterEntryPoint<CoreFlow>();
        }
    }
}