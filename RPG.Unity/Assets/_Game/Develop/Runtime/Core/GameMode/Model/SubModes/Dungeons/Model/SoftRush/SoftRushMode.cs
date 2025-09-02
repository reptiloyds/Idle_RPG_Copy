using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement.Forecast;
using PleasantlyGames.RPG.Runtime.Core.Ally;
using PleasantlyGames.RPG.Runtime.Core.Companion;
using PleasantlyGames.RPG.Runtime.Core.Enemy;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Contract;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.Definition;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.View;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Health;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = System.Random;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush
{
    public class SoftRushMode : DungeonMode, IAssetUser, ITickable, IResourcefulDungeon
    {
        [Inject] private LocationFactory _locationFactory;
        [Inject] private SoftRushDataProvider _dataProvider;
        [Inject] private SoftRushConfiguration _configuration;
        
        [Inject] private AllySquad _allySquad;
        [Inject] private EnemySquad _enemySquad;
        [Inject] private CompanionSquad _companionSquad;
        [Inject] private ItemSkillService _itemSkillService;
        [Inject] private CharacterSkillService _characterSkillService;
        [Inject] private LocationUnitCollection _locationUnits;
        [Inject] private IWindowService _windowService;
        
        private int _launchLevel;
        private ManualStatsSheet<SoftRushSheet> _manualStatsSheet;
        private readonly List<ManualStatData<UnitStatType>> _manualStats = new(2);
        
        private SoftRushDataContainer _data;
        private Sprite _rewardSprite;
        private BaseValueFormula _rewardFormula;
        
        private bool _isLeaveTimer;
        private float _leaveTimer;

        private UnitHealth _bossHealth;
        private BigDouble.Runtime.BigDouble _lootedResourceAmount;
        private BigDouble.Runtime.BigDouble _targetResourceAmount;
        
        private const float LoseLeaveDelay = 1f;
        private SoftRushSheet.Row _sheet;
        private SoftRushSheet.Row Sheet => _sheet ??= Balance.Get<SoftRushSheet>()[0];
        
        protected override SubModeSetup Setup => _configuration.SubModeSetup;
        protected override SubModeDataContainer SubModeData => _data.SubModeData;
        protected override string SubModeId => "SoftRush";
        
        private readonly SerialDisposable _loseTimer = new();
        private readonly ReactiveProperty<float> _loseDelay = new();
        public ReadOnlyReactiveProperty<float> LoseDelay => _loseDelay;

        public override GameModeType Type => GameModeType.SoftRush;
        public override Sprite RewardImage => _rewardSprite;
        public Color RewardColor => _configuration.RewardColor;
        public ResourceType RewardType => _configuration.RewardType;
        public int MaxLevel => _sheet.MaxLevel;
        public bool IsAutoSweep => _data.IsAutoSweep;
        public int LaunchLevel => _launchLevel;
        public int AvailableLevel => _data.AvailableLevel;
        public BigDouble.Runtime.BigDouble LootedResourceAmount => _lootedResourceAmount;

        public event Action OnLootChanged;
        
        [Preserve]
        public SoftRushMode()
        {
            
        }

        public override void Initialize()
        {
            _sheet = Balance.Get<SoftRushSheet>()[0];
            _manualStatsSheet = Balance.Get<ManualStatsSheet<SoftRushSheet>>();
            
            _data = _dataProvider.GetData();
            _rewardSprite = ResourceService.GetResource(_configuration.RewardType)?.Sprite;
            _launchLevel = _data.AvailableLevel;
            Name = GetFormattedFullName();
            
            if (_sheet.RewardFormulaType == FormulaType.CustomSheet)
            {
                var manualRewardSheet = Balance.Get<ManualFormulaSheet<SoftRushSheet, string>>();
                _rewardFormula = manualRewardSheet.GetValueFormula("Rewards");
            }
            else
                _rewardFormula = _sheet.RewardFormulaType.CreateFormula(_sheet.RewardFormulaJSON);
            
            base.Initialize();
        }

        public override async UniTask Select() => 
            await _windowService.OpenAsync<SoftRushLaunchWindow>();

        public override string GetFormattedFullName() => 
            $"{GetFormattedModeName()} {_data.AvailableLevel}";
        
        public void SetAutoSweep(bool value) => 
            _data.IsAutoSweep = value;
        
        public void Sweep(int level, int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                ApplyRewardFor(level);
                SpendEnterResource();
            }
        }

        public void ApplyRewardFor(int level) => 
            ResourceService.AddResource(RewardType, GetRewardFor(level));
        
        public BigDouble.Runtime.BigDouble GetRewardFor(int level) => 
            _rewardFormula.CalculateBigDouble(level);
        
        public void SetLaunchLevel(int level)
        {
            if(level < 1 || level > AvailableLevel) return;
            _launchLevel = level;
        }
        
        public override async UniTask LaunchAsync()
        {
            await _locationFactory.CreateAsync(_sheet.LocationList[0]);
            _locationFactory.Location.StopMovement();
            
            _allySquad.SetSpawn(_locationFactory.Location.AllySpawn);
            _companionSquad.SetSpawn(_locationFactory.Location.CompanionSpawn);
            _enemySquad.SetSpawn(_locationFactory.Location.SoftRushSpawn);
            
            _allySquad.MergeHealth();

            await UniTask.WhenAll(_allySquad.SpawnUnitsAsync(), _companionSquad.SpawnUnitsAsync());

            _loseDelay.Value = _configuration.AutoLoseTimeInSeconds;
            Time.LaunchLocalTimer(_loseTimer, _loseDelay, Lose);
            _lootedResourceAmount = 0;

            _targetResourceAmount = GetRewardFor(_launchLevel);

            _locationUnits.OnUnitEnter += OnLocalUnitEnter;
            _itemSkillService.UnlockAutoCast();
            _itemSkillService.ResetAllSkill();
            _characterSkillService.UnlockAutoCast();
            _characterSkillService.ResetAllSkill();
            await CreateEnemyAsync();
            base.LaunchAsync();
        }
        
        public override void Dispose()
        {
            CleanUp();
            
            base.Dispose();
        }
        
        private void CleanUp()
        {
            _loseTimer.Disposable?.Dispose();
            _isLeaveTimer = false;
            
            _enemySquad.OnAllUnitsDead -= WinBoss;
            _allySquad.OnAllUnitsDead -= Lose;
            _locationUnits.OnUnitEnter -= OnLocalUnitEnter;
            
            if (_bossHealth != null) 
                _bossHealth.OnDamage -= OnBossDamaged;
            
            _allySquad.Clear();
            _companionSquad.Clear();
            _enemySquad.Clear();
            
            _itemSkillService.BlockAutoCast();
            _itemSkillService.StopSkills();
            _characterSkillService.BlockAutoCast();
            _characterSkillService.StopSkills();
        }
        
        private async UniTask CreateEnemyAsync()
        {
            _enemySquad.OnAllUnitsDead += WinBoss;
            _allySquad.OnAllUnitsDead += Lose;
            
            if (_manualStatsSheet.Contains(_launchLevel))
            {
                var statModifiers = _manualStatsSheet[_launchLevel];
                foreach (var elem in statModifiers) 
                    _manualStats.Add(elem.ManualStatData);
            }
            
            var boss = await _enemySquad.SpawnBossAsync(_sheet.UnitId, _configuration.BossHealthBarSize, _launchLevel, _manualStats);
            _manualStats.Clear();
            
            _bossHealth = boss.Health;
            _bossHealth.OnDamage += OnBossDamaged;
            
            _enemySquad.MoveForward();
        }

        private void OnBossDamaged(BigDouble.Runtime.BigDouble damage)
        {
            _lootedResourceAmount = (1 - _bossHealth.ProgressValue) * _targetResourceAmount;
            OnLootChanged?.Invoke();
        }
        
        private void OnLocalUnitEnter(UnitView unitView)
        {
            if (unitView.TeamType != _enemySquad.TeamType) return;
            
            _locationUnits.OnUnitEnter -= OnLocalUnitEnter;
            StartCombat();
        }

        private void StartCombat()
        {
            _allySquad.Fight();
            _companionSquad.Fight();
            _enemySquad.Fight();
        }
        
        private void WinBoss()
        {
            _enemySquad.OnAllUnitsDead -= WinBoss;
            _allySquad.OnAllUnitsDead -= Lose;
            
            _allySquad.MoveToSpawn(UnitPointKey.Spawn);
            _companionSquad.MoveToSpawn(UnitPointKey.Spawn);
            Win();
        }
        
        protected override void Win()
        {
            _loseTimer.Disposable?.Dispose();
            _data.AvailableLevel++;
            Name = GetFormattedFullName();
            if (_bossHealth != null) 
                _bossHealth.OnDamage -= OnBossDamaged;
            base.Win();
        }
        
        protected override void Lose()
        {
            _loseTimer.Disposable?.Dispose();
            _enemySquad.OnAllUnitsDead -= WinBoss;
            _allySquad.OnAllUnitsDead -= Lose;
            
            _allySquad.Idle();
            _companionSquad.Idle();
            _enemySquad.Idle();
            base.Lose();
            _isLeaveTimer = true;
            _leaveTimer = LoseLeaveDelay;
        }

        public override void Leave()
        {
            _isLeaveTimer = false;
            RequestExit();
        }

        public void Tick()
        {
            if(!IsLaunched) return;

            if (!_isLeaveTimer) return;
            _leaveTimer -= UnityEngine.Time.deltaTime;
            if (_leaveTimer > 0) return;
            _isLeaveTimer = false;
            RequestExit();
        }

        public event Action OnNeedsChanged;
        
        public void FillNeeds(in Dictionary<AssetType, HashSet<string>> needs)
        {
            var unitSheet = Balance.Get<UnitsSheet>();
            needs[AssetType.Unit].Add(unitSheet[Sheet.UnitId].GetPrefabId(0));
            needs[AssetType.Location].Add(Sheet.LocationList[0]);
        }
    }
}