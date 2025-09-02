using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.AssetManagement.Forecast;
using PleasantlyGames.RPG.Runtime.Core.Ally;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Camera;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Collections.Model;
using PleasantlyGames.RPG.Runtime.Core.Companion;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model;
using PleasantlyGames.RPG.Runtime.Core.Effects.Model;
using PleasantlyGames.RPG.Runtime.Core.Enemy;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.Music.Model;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Model;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.View.Mediator;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.View;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Model;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Factories;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Services;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Service;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using PleasantlyGames.RPG.Runtime.Core.PopupNumbers.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Offers.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Offers.View.Mediator;
using PleasantlyGames.RPG.Runtime.Core.Projectile.Model;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Service;
using PleasantlyGames.RPG.Runtime.Core.Story;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Core.UnitRendering;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Health;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.Runtime.VIP.Contract;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Core
{
    public class CoreFlow : IInitializable, ILoadUnit
    {
        [Inject] private LoadingScreenProvider _loadingScreenProvider;
        [Inject] private UIFactory _uiFactory;
        
        //[Inject] private ProjectileFactory _projectileFactory;
        [Inject] private HealthBarFactory _healthBarFactory;
        [Inject] private EffectFactory _effectFactory;
        [Inject] private PopupTextFactory _popupTextFactory;
        [Inject] private PopupResourceFactory _popupResourceFactory;
        [Inject] private SkillViewFactory _skillViewFactory;
        //[Inject] private StuffInventory _stuffInventory;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private IWindowService _windowService;
        
        [Inject] private ISaveService _save;
        [Inject] private ResourceService _resourceService;
        [Inject] private GlobalStatProvider _globalStatProvider;
        [Inject] private ProjectileFactory _projectileFactory;
        [Inject] private ContentService _content;
        [Inject] private BranchService _branchService;
        [Inject] private CameraPositioner _cameraPositioner;
        [Inject] private UnitStatService _unitStatService;
        [Inject] private CharacterService _characterService;
        [Inject] private SkillInventory _skillInventory;
        [Inject] private SkillFactory _skillFactory;
        [Inject] private ItemSkillService _itemSkill;
        [Inject] private CharacterSkillService _characterSkill;
        [Inject] private CompanionInventory _companionInventory;
        [Inject] private CompanionSquad _companionSquad;
        [Inject] private AllySquad _allySquad;
        [Inject] private EnemySquad _enemySquad;
        [Inject] private GameModeSwitcher _gameModeSwitcher;
        [Inject] private DungeonModeFacade _dungeonFacade;
        [Inject] private StuffInventory _stuffInventory;
        [Inject] private StatImprover _statImprover;
        [Inject] private LootboxService _lootboxService;
        [Inject] private ProductService _product;
        [Inject] private PassiveIncomeModel _passiveIncome;
        [Inject] private UnitRenderer _unitRenderer;
        [Inject] private QuestService _quest;
        [Inject] private DailyRoulette _dailyRoulette;
        [Inject] private RouletteFacade _rouletteFacade;
        [Inject] private CollectionService _collection;
        [Inject] private PeriodicRewardService _periodicReward;
        [Inject] private TutorialService _tutorial;
        [Inject] private BlessingService _blessing;
        [Inject] private StoryService _storyService;
        [Inject] private MusicLauncher _musicLauncher;
        [Inject] private ProductOfferService _productOfferService;
        [Inject] private DailyService _dailyService;
        [Inject] private IAdService _adService;
        [Inject] private AssetForecaster _assetForecaster;
        [Inject] private ChatService _chatService;
        [Inject] private PiggyBankService _piggyBankService;
        [Inject] private PiggyBankBonusService _piggyBankBonusService;
        [Inject] private PiggyBankBonusViewFactory _piggyBankBonusViewFactory;
        [Inject] private GalleryService _galleryService;
        [Inject] private IAnalyticsService _analytics;
        [Inject] private StageRewardsService _stageRewardsService;
        [Inject] private VipService _vipService;
        
        [Inject] private UIDailyLoginRewardMediator _dailyLoginRewardMediator;
        [Inject] private UIPassiveIncomeMediator _passiveIncomeMediator;
        [Inject] private UIProductOfferMediator _productOfferMediator;
        [Inject] private ResourceViewService _resourceViewService;
        
        [Inject] private TooltipFactory _tooltipFactory;

        [Preserve]
        public CoreFlow()
        {
        } 
        
        public async void Initialize()
        {
            var queue = new Queue<ILoadUnit>();
            queue.Enqueue(this);

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            
            try
            {
                await _loadingScreenProvider.Load(queue, token);
                _loadingScreenProvider.Hide().Forget();
                _analytics.SendCoreSceneLoaded();
                _analytics.SendLastPurchase();
            }
            catch (OperationCanceledException canceledException)
            {
                _loadingScreenProvider.Hide().Forget();
            }
        }

        public string DescriptionToken => "CoreInitialization";
        
        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.3f);
            var tasks = new List<UniTask>
            {
                _healthBarFactory.WarmUpAsync(),
                _projectileFactory.WarmUpAsync(),
                _effectFactory.WarmUpAsync(),
                _popupTextFactory.WarmUpAsync(),
                _popupResourceFactory.WarmUpAsync(),
                _skillViewFactory.WarmUpAsync(),
                _stuffInventory.WarmUpAsync(),
                _spriteProvider.WarmUpAsync(),
                _windowService.WarmUpAsync(),
                _piggyBankBonusViewFactory.WarmUpAsync(),
                _tooltipFactory.WarmUpAsync()
            };
            
            await UniTask.WhenAll(tasks);
            progress?.Report(0.5f);
            
            _save.Initialize();
            _adService.Initialize();
            _resourceService.Initialize();
            _content.Initialize();
            _projectileFactory.Initialize();
            _globalStatProvider.Initialize();
            _branchService.Initialize();
            _cameraPositioner.Initialize();
            _unitStatService.Initialize();
            _skillFactory.Initialize();
            _characterService.Initialize();
            _skillInventory.Initialize();
            _itemSkill.Initialize();
            _characterSkill.Initialize();
            _companionInventory.Initialize();
            _companionSquad.Initialize();
            _allySquad.Initialize();
            _enemySquad.Initialize();
            _gameModeSwitcher.Initialize(GameModeType.Main);
            _dungeonFacade.Initialize();
            _stuffInventory.Initialize();
            _statImprover.Initialize();
            _lootboxService.Initialize();
            _product.InitializeAsync().Forget();
            _collection.Initialize();
            _dailyRoulette.Initialize();
            _rouletteFacade.Initialize();
            _periodicReward.Initialize();
            _blessing.Initialize();
            _quest.Initialize();
            
            _productOfferService.Initialize();
            _dailyService.Initialize();
            _assetForecaster.Initialize();
            _chatService.Initialize();
            _galleryService.Initialize();
            _vipService.Initialize();
            _passiveIncome.Initialize();

            await InitializeUI();
            await _dailyLoginRewardMediator.InitializeAsync();
            await _passiveIncomeMediator.InitializeAsync();
            await _productOfferMediator.InitializeAsync();
            await _resourceViewService.InitializeAsync();
#if RPG_DEV && STORY_DISABLED
            await InitializeSubmodules();
#else
            await _storyService.InitializeAsync(InitializeSubmodules);
#endif
            await _assetForecaster.Forecast();
            
            await _gameModeSwitcher.SwitchToAsync(_gameModeSwitcher.ModeType);
            
            _unitRenderer.Initialize();
            _piggyBankService.Initialize();
            _piggyBankBonusService.Initialize();
            
            _stageRewardsService.Initialize();
            _tooltipFactory.Initialize();
        }

        private async UniTask InitializeUI()
        {
            var uiRootPrefab = await _uiFactory.LoadAsync(Asset.UI.CoreRoot, false);
            var uiRoot = Object.Instantiate(uiRootPrefab);
            await uiRoot.GetComponent<CoreUIScope>().InitializeAsync();
        }
        
        private async UniTask InitializeSubmodules()
        {
            await _tutorial.InitializeAsync();
            _musicLauncher.Initialize();
        }
    }
}