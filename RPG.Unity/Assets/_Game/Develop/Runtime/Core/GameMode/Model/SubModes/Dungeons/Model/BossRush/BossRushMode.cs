using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement.Forecast;
using PleasantlyGames.RPG.Runtime.Core.Ally;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Companion;
using PleasantlyGames.RPG.Runtime.Core.Enemy;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Contract;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Definition;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.View;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Type;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = System.Random;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush
{
    public class BossRushMode : DungeonMode, IAssetUser, ITickable, IResourcefulDungeon, IDisposable
    {
        [Inject] private LocationFactory _locationFactory;
        [Inject] private BossRushDataProvider _dataProvider;
        [Inject] private BossRushConfiguration _configuration;
        
        [Inject] private AllySquad _allySquad;
        [Inject] private EnemySquad _enemySquad;
        [Inject] private CompanionSquad _companionSquad;
        [Inject] private ItemSkillService _itemSkillService;
        [Inject] private CharacterSkillService _characterSkillService;
        [Inject] private LocationUnitCollection _locationUnits;
        [Inject] private IWindowService _windowService;
        
        private ManualStatsSheet<BossRushSheet> _manualStatsSheet;
        private readonly List<ManualStatData<UnitStatType>> _manualStats = new(2);
        private BossRushDataContainer _data;
        private BossRushSheet.Row _sheet;
        public BossRushSheet.Row Sheet => _sheet ??= Balance.Get<BossRushSheet>()[0];
        
        protected override SubModeSetup Setup => _configuration.SubModeSetup;
        protected override SubModeDataContainer SubModeData => _data.SubModeData;
        protected override string SubModeId => "BossRush";
        public override GameModeType Type => GameModeType.BossRush;
        public override Sprite RewardImage => _rewardSprite;
        
        private StatModifier _locationSpeedModifier;
        private int _bossIndex;
        private int _launchLevel;
        private Sprite _rewardSprite;
        private BaseValueFormula _rewardFormula;
        
        private bool _isMoveToEnemy;
        private float _movementTimer;

        private bool _isLeaveTimer;
        private float _leaveTimer;
        
        private float SwitchWaveDuration => _configuration.SwitchWaveDuration;
        
        private const string StatModifierName = "BossRush";
        private const float LoseLeaveDelay = 1f;
        
        private readonly SerialDisposable _loseTimer = new();
        private readonly ReactiveProperty<float> _loseDelay = new();
        public ReadOnlyReactiveProperty<float> LoseDelay => _loseDelay;

        public int LaunchLevel => _launchLevel;
        public int AvailableLevel => _data.AvailableLevel;
        
        public int DeadBossAmount => _bossIndex;
        public int MaxBossAmount => _sheet.UnitAmount;
        public int MaxLevel => _sheet.MaxLevel;
        public bool IsAutoSweep => _data.IsAutoSweep;
        public Color RewardColor => _configuration.RewardColor;
        public ResourceType RewardType => _configuration.RewardType;
        
        public event Action OnBossDefeated;

        [Preserve]
        public BossRushMode()
        {
            
        }

        public override void Initialize()
        {
            _sheet = Balance.Get<BossRushSheet>()[0];
            _manualStatsSheet = Balance.Get<ManualStatsSheet<BossRushSheet>>();
            
            _data = _dataProvider.GetData();
            _rewardSprite = ResourceService.GetResource(_configuration.RewardType)?.Sprite;
            _launchLevel = _data.AvailableLevel;
            Name = GetFormattedFullName();

            if (_sheet.RewardFormulaType == FormulaType.CustomSheet)
            {
                var manualRewardSheet = Balance.Get<ManualFormulaSheet<BossRushSheet, string>>();
                _rewardFormula = manualRewardSheet.GetValueFormula("Rewards");
            }
            else
                _rewardFormula = _sheet.RewardFormulaType.CreateFormula(_sheet.RewardFormulaJSON);
            
            base.Initialize();
        }
        
        public override async UniTask Select() => 
            await _windowService.OpenAsync<BossRushLaunchWindow>();

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
            _bossIndex = 0;
            
            await _locationFactory.CreateAsync(_sheet.LocationList[0]);
            
            _allySquad.SetSpawn(_locationFactory.Location.AllySpawn);
            _companionSquad.SetSpawn(_locationFactory.Location.CompanionSpawn);
            _enemySquad.SetSpawn(_locationFactory.Location.EnemySpawn);
            
            _allySquad.MergeHealth();
            await UniTask.WhenAll(_allySquad.SpawnUnitsAsync(), _companionSquad.SpawnUnitsAsync());
            
            _locationSpeedModifier = new StatModifier(_configuration.BonusUnitSpeed, StatModType.Flat, StatModifierName, GroupOrder.None);
            
            _allySquad.OnAchieveMovementTarget += OnAllyAchieveMovementTarget;
            _companionSquad.OnAchieveMovementTarget += OnAllyAchieveMovementTarget;
            
            WalkToEnemy();
            
            _loseDelay.Value = _configuration.AutoLoseTimeInSeconds;
            Time.LaunchLocalTimer(_loseTimer, _loseDelay, Lose);
            
            _itemSkillService.ResetAllSkill();
            _itemSkillService.UnlockAutoCast();
            _characterSkillService.ResetAllSkill();
            _characterSkillService.UnlockAutoCast();
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
            _isMoveToEnemy = false;
            _isLeaveTimer = false;
            
            _allySquad.OnAchieveMovementTarget -= OnAllyAchieveMovementTarget;
            _companionSquad.OnAchieveMovementTarget -= OnAllyAchieveMovementTarget;
            _enemySquad.OnAllUnitsDead -= WinBoss;
            _allySquad.OnAllUnitsDead -= Lose;
            _locationUnits.OnUnitEnter -= OnLocalUnitEnter;
            
            _allySquad.Clear();
            _companionSquad.Clear();
            _enemySquad.Clear();
            
            _itemSkillService.BlockAutoCast();
            _itemSkillService.StopSkills();
            _characterSkillService.BlockAutoCast();
            _characterSkillService.StopSkills();
        }

        void ITickable.Tick()
        {
            if(!IsLaunched) return;

            if (_isMoveToEnemy)
            {
                _movementTimer -= UnityEngine.Time.deltaTime;
                if (_movementTimer <= 0)
                {
                    _isMoveToEnemy = false;
                    CreateEnemyAsync().Forget();
                }
            }

            if (!_isLeaveTimer) return;
            _leaveTimer -= UnityEngine.Time.deltaTime;
            if (_leaveTimer > 0) return;
            _isLeaveTimer = false;
            RequestExit();
        }

        protected override void Win()
        {
            _loseTimer.Disposable?.Dispose();
            _data.AvailableLevel++;
            Name = GetFormattedFullName();
            base.Win();
        }

        protected override void Lose()
        {
            _loseTimer.Disposable?.Dispose();
            _enemySquad.OnAllUnitsDead -= WinBoss;
            _allySquad.OnAllUnitsDead -= Lose;
            _locationUnits.OnUnitEnter -= OnLocalUnitEnter;
            
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

        private void WalkToEnemy()
        {
            _allySquad.SimulateMovement();
            _companionSquad.SimulateMovement();
            
            _locationFactory.Location.StartMovement(_configuration.LocationSpeed);

            _isMoveToEnemy = true;
            _movementTimer = SwitchWaveDuration;
        }

        private async UniTask CreateEnemyAsync()
        {
            _enemySquad.OnAllUnitsDead += WinBoss;
            _allySquad.OnAllUnitsDead += Lose;
            _locationUnits.OnUnitEnter += OnLocalUnitEnter;

            if (_manualStatsSheet.Contains(_launchLevel))
            {
                var statModifiers = _manualStatsSheet[_launchLevel];
                foreach (var elem in statModifiers)
                {
                    if (elem.Type == UnitStatType.Health)
                    {
                        var bigDouble = new BigDouble.Runtime.BigDouble(elem.ManualStatData.M, elem.ManualStatData.E);
                        bigDouble /= _sheet.UnitAmount;
                        _manualStats.Add(new ManualStatData<UnitStatType>()
                        {
                            Type = UnitStatType.Health,
                            M = bigDouble.mantissa,
                            E = bigDouble.exponent,
                        });
                    }
                    else
                        _manualStats.Add(elem.ManualStatData);
                } 
            }
            
            await _enemySquad.SpawnBossAsync(_sheet.Units.GetRandomElement(), _configuration.BossHealthBarSize, _launchLevel, _manualStats);  
            _manualStats.Clear();

            _enemySquad.AddModifier(UnitStatType.MoveSpeed, _locationSpeedModifier);
            _enemySquad.MoveForward();
        }
        
        private void OnLocalUnitEnter(UnitView unitView)
        {
            if (unitView.TeamType != _enemySquad.TeamType) return;
            
            _locationUnits.OnUnitEnter -= OnLocalUnitEnter;
            StartCombat();
        }

        private void StartCombat()
        {
            _enemySquad.RemoveModifier(UnitStatType.MoveSpeed, _locationSpeedModifier);
            _locationFactory.Location.StopMovement();
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
            _bossIndex++;
            
            OnBossDefeated?.Invoke();
        }

        private void OnAllyAchieveMovementTarget()
        {
            if(_allySquad.HasAnyMovementTarget() || _companionSquad.HasAnyMovementTarget()) return;
            
            ExecuteAllyWinActions();

            if (_bossIndex >= MaxBossAmount)
                Win();
            else
                WalkToEnemy();
        }

        private void ExecuteAllyWinActions()
        {
            foreach (var actionType in _configuration.ActionAfterAllyWin)
                ExecuteAllyAction(actionType);
        }

        private void ExecuteAllyAction(SquadActionType actionType)
        {
            switch (actionType)
            {
                case SquadActionType.FullHealth:
                    _allySquad.FullHealth();
                    break;
            }
        }
        
        public event Action OnNeedsChanged;

        public void FillNeeds(in Dictionary<AssetType, HashSet<string>> needs)
        {
            var unitSheet = Balance.Get<UnitsSheet>();
            var unitHashSet = needs[AssetType.Unit];
            foreach (var unit in Sheet.Units) 
                unitHashSet.Add(unitSheet[unit].GetPrefabId(0));
            needs[AssetType.Location].Add(Sheet.LocationList[0]);
        }
    }
}